using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeatSaberScore.Model
{
    internal interface IStepExecuter
    {
        public enum Status
        {
            Processing,
            Completed,
            Failed,
        }

        public int TotalStepCount { get; }

        public int FinishedStepCount { get; }

        public Status CurrentStatus { get; }

        public Status ExecuteStep();
    }
}
