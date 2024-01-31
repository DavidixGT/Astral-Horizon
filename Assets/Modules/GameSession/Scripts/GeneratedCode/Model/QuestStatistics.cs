//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System.Collections.Generic;
using Session.Utils;

namespace Session.Model
{
	public readonly partial struct QuestStatistics
	{
		private readonly int _completionCount;
		private readonly int _failureCount;
		private readonly long _lastStartTime;
		private readonly long _lastCompletionTime;

		public QuestStatistics(IDataChangedCallback parent)
		{
			_completionCount = default(int);
			_failureCount = default(int);
			_lastStartTime = default(long);
			_lastCompletionTime = default(long);
		}

		public QuestStatistics(SessionDataReader reader, IDataChangedCallback parent)
		{
			_completionCount = reader.ReadInt(EncodingType.EliasGamma);
			_failureCount = reader.ReadInt(EncodingType.EliasGamma);
			_lastStartTime = reader.ReadLong(EncodingType.EliasGamma);
			_lastCompletionTime = reader.ReadLong(EncodingType.EliasGamma);
		}

		public int CompletionCount => _completionCount;
		public int FailureCount => _failureCount;
		public long LastStartTime => _lastStartTime;
		public long LastCompletionTime => _lastCompletionTime;

        public void Serialize(SessionDataWriter writer)
        {
			writer.WriteInt(_completionCount, EncodingType.EliasGamma);
			writer.WriteInt(_failureCount, EncodingType.EliasGamma);
			writer.WriteLong(_lastStartTime, EncodingType.EliasGamma);
			writer.WriteLong(_lastCompletionTime, EncodingType.EliasGamma);
        }

        public bool Equals(QuestStatistics other)
        {
			if (CompletionCount != other.CompletionCount) return false;
			if (FailureCount != other.FailureCount) return false;
			if (LastStartTime != other.LastStartTime) return false;
			if (LastCompletionTime != other.LastCompletionTime) return false;
            return true;
        }

        public struct EqualityComparer : IEqualityComparer<QuestStatistics>
        {
            public bool Equals(QuestStatistics first, QuestStatistics second) => first.Equals(second);
            public int GetHashCode(QuestStatistics obj) => obj.GetHashCode();
        }
	}
}
