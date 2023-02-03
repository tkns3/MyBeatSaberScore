using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;

namespace MyBeatSaberScore.Utility
{
    // ＜前提＞
    //   リリースのタグ名は"v*.*.*"の形式で指定しなければならない。
    //   パッケージバージョンは"*.*.*"の形式で指定しなければならない。
    // ＜参考＞
    // アセンブリバージョン 形式は"*.*.*.*"
    //   Assembly.GetExecutingAssembly().GetName().Version
    // ファイルバージョン 形式は"*.*.*.*"
    //   Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version
    // パッケージバージョン
    //   Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion

    class Updater
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly string Owner = "tkns3";
        private static readonly string Repo = "MyBeatSaberScore";
        private static readonly string OriginalExeName = "MyBeatSaberScore.exe";
        private static readonly string ApiReleasesURL = $"https://api.github.com/repos/{Owner}/{Repo}/releases";

        private static string[] _arguments = Array.Empty<string>();

        public static Version? CurrentVersion { get; private set; } = new Version(Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0");
        public static Version? LatestVersion { get; private set; }
        public static string ExeDir { get; private set; } = "";
        public static string ExeName { get; private set; } = "";
        public static string ExePath { get; private set; } = "";
        public static string NewExeName { get; private set; } = "";
        public static string NewExePath { get; private set; } = "";
        public static string OldExeName { get; private set; } = "";
        public static string OldExePath { get; private set; } = "";
        public static List<Release> ReleasesCache { get; private set; } = new();
        public static bool IsExistNewVersion { get; private set; } = false;

        public static void Initialize(string[] args)
        {
            ReleasesCache.Clear();

            _arguments = new string[args.Length];
            args.CopyTo(_arguments, 0);

            ExePath = Environment.ProcessPath ?? "";
            ExeDir = Path.GetDirectoryName(ExePath) ?? "";
            ExeName = Path.GetFileName(ExePath);

            if (ExeName.Length > 8 && ExeName[^8..].Equals(".old.exe"))
            {
                NewExeName = $"{ExeName[0..^8]}.exe";
                OldExeName = ExeName;
            }
            else if (ExeName.Length > 4)
            {
                NewExeName = ExeName;
                OldExeName = $"{ExeName[0..^4]}.old.exe";
            }

            NewExePath = Path.Combine(ExeDir, NewExeName);
            OldExePath = Path.Combine(ExeDir, OldExeName);

            // *.exe *.old.exe -> 対応
            // ある  起動      -> *.exeを起動
            // ない  起動      -> *.old.exeを*.exeにリネームして*.exeを起動
            // 起動  ある      -> *.old.exeを削除
            // 起動  ない      -> なにもしない
            {
                if (ExePath.Equals(OldExePath))
                {
                    if (!File.Exists(NewExePath))
                    {
                        File.Move(OldExePath, NewExePath);
                    }
                    RunNew(NewExePath);
                }

                if (ExePath.Equals(NewExePath) && File.Exists(OldExePath))
                {
                    foreach (var arg in args)
                    {
                        if (arg.StartsWith("--old-pid="))
                        {
                            int pid = int.Parse(args[0][10..]);
                            var p = Process.GetProcessById(pid);
                            p.WaitForExit(1000);
                        }
                    }
                    try
                    {
                        File.Delete(OldExePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn($"{OldExePath}: {ex}");
                    }
                }
            }
        }

        public static async Task FetchReleasesAsync()
        {
            ReleasesCache.Clear();
            IsExistNewVersion = false;

            try
            {
                _logger.Info(ApiReleasesURL);
                var releases = await HttpTool.GetAndDeserialize<List<Release>>(ApiReleasesURL);
                if (releases != null)
                {
                    ReleasesCache = releases;
                    if (ReleasesCache.Count > 0)
                    {
                        var tag_name = ReleasesCache[0].tag_name;
                        if (tag_name != null && tag_name.Length > 1)
                        {
                            LatestVersion = new Version(tag_name[1..]);
                            IsExistNewVersion = LatestVersion > CurrentVersion;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{ApiReleasesURL}: {ex}");
            }
        }

        public static async Task StartUpdate()
        {
            if (ExePath.Length == 0)
            {
                return;
            }

            if (!IsExistNewVersion)
            {
                return;
            }

            if (ReleasesCache.Count == 0)
            {
                return;
            }

            var assets = ReleasesCache[0].assets;
            if (assets == null)
            {
                return;
            }

            string? downloadLink = Array.Find(assets, asset => OriginalExeName.Equals(asset.name))?.browser_download_url;
            if (string.IsNullOrEmpty(downloadLink))
            {
                // Githubに取得対象が存在しない
                return;
            }

            try
            {
                if (File.Exists(OldExePath))
                {
                    File.Delete(OldExePath);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"{OldExePath}: {ex}");
                return;
            }

            var nexExeTmpPath = $"{NewExePath}.tmp";
            try
            {
                _logger.Info(downloadLink);
                await HttpTool.Download(downloadLink, nexExeTmpPath);
            }
            catch (Exception ex)
            {
                _logger.Warn($"{downloadLink}: {ex}");
                return;
            }

            File.Move(ExePath, OldExePath);
            File.Move(nexExeTmpPath, NewExePath);
            RunNew(NewExePath);
        }

        private static void RunNew(string newExePath)
        {
            string[] args = new string[_arguments.Length + 1];
            _arguments.CopyTo(args, 0);
            args[_arguments.Length] = $"--old-pid={Environment.ProcessId}";
            Process.Start(newExePath, args);
            System.Windows.Application.Current.Dispatcher.Invoke(() => { System.Windows.Application.Current.Shutdown(); });
        }
    }

    public class Release
    {
        public string? url { get; set; }
        public string? assets_url { get; set; }
        public string? upload_url { get; set; }
        public string? html_url { get; set; }
        public int id { get; set; }
        public string? node_id { get; set; }
        public string? tag_name { get; set; }
        public string? target_commitish { get; set; }
        public string? name { get; set; }
        public bool draft { get; set; }
        public User? author { get; set; }
        public bool? prerelease { get; set; }
        public string? created_at { get; set; }
        public string? published_at { get; set; }
        public Asset[]? assets { get; set; }
        public string? tarball_url { get; set; }
        public string? zipball_url { get; set; }
        public string? body { get; set; }

        public class Asset
        {
            public string? url { get; set; }
            public int id { get; set; }
            public string? node_id { get; set; }
            public string? name { get; set; }
            public string? label { get; set; }
            public User? uploader { get; set; }
            public string? content_type { get; set; }
            public string? state { get; set; }
            public int size { get; set; }
            public string? created_at { get; set; }
            public string? updated_at { get; set; }
            public string? browser_download_url { get; set; }
        }

        public class User
        {
            public string? login { get; set; }
            public int id { get; set; }
            public string? node_id { get; set; }
            public string? avatar_url { get; set; }
            public string? gravatar_id { get; set; }
            public string? url { get; set; }
            public string? html_url { get; set; }
            public string? followers_url { get; set; }
            public string? following_url { get; set; }
            public string? gists_url { get; set; }
            public string? starred_url { get; set; }
            public string? subscriptions_url { get; set; }
            public string? organizations_url { get; set; }
            public string? repos_url { get; set; }
            public string? events_url { get; set; }
            public string? received_events_url { get; set; }
            public string? type { get; set; }
            public bool site_admin { get; set; }

        }
    }
}
