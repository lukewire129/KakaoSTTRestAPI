using KakaoSTTRestAPI.Model;
using System;

namespace KakaoSTTRestAPI.Interfaces
{
        public interface IRecorder
        {
                IRecorder SetApiKey(string _apiKey);
                void StartRecording();
                void RecordEnd();
                Action<ResultMessage> resultAction { get; set; }
        }
}
