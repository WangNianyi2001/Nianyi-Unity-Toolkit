using UnityEngine;

namespace Nianyi {
	public static class Debug {
		public enum LogType { Message, Warning, Error }
		public static void PrintLog(object msg, LogType type = LogType.Message)
			=> PrintLog(msg.ToString(), type);
		public static void PrintLog(string msg, LogType type = LogType.Message) {
			switch(type) {
				case LogType.Message:
                    UnityEngine.Debug.Log(msg);
					break;
				case LogType.Warning:
                    UnityEngine.Debug.LogWarning(msg);
					break;
				case LogType.Error:
                    UnityEngine.Debug.LogError(msg);
					break;
			}
		}

		public static YieldInstruction WaitForSeconds(float seconds) 
			=> new WaitForSeconds(seconds);
	}
}
