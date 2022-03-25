using RestSharp;
using KakaoSTTRestAPI.Interfaces;
using KakaoSTTRestAPI.Model;
using System.Text.Json;

namespace KakaoSTTRestAPI.Helper
{
        public class baseRecoder:IRecorder
        {
                protected string fileName = "test.wav";

                public System.Action<ResultMessage> resultAction { get; set; }
                protected string apiKey { get; set; }
                protected byte[] data { get; set; }

                public virtual void RecordEnd()
                {
                        throw new System.NotImplementedException();
                }

                public virtual IRecorder SetApiKey(string _apiKey)
                {
                        throw new System.NotImplementedException();
                }

                public virtual void StartRecording()
                {
                        throw new System.NotImplementedException();
                }

                protected void GetResultText()
                {
                        if (string.IsNullOrEmpty(apiKey)) 
                                this.resultAction?.Invoke(new ResultMessage()
                                {
                                        resultCode = Enums.ResultCode.APIKEYISNULL,
                                        message = null
                                });

                        if(data == null )
                                this.resultAction?.Invoke(new ResultMessage()
                                {
                                        resultCode = Enums.ResultCode.VOICEMEMORYDATAISNULL,
                                        message = null
                                });

                        var Client = new RestClient("https://kakaoi-newtone-openapi.kakao.com/v1/recognize");
                        var request = new RestRequest()
                                          .AddHeader("Transfer-Encoding", "chunked")
                                          .AddHeader("Authorization", $"KakaoAK {apiKey}")
                                          .AddFile(fileName, data, fileName);
                        request.RequestFormat = RestSharp.DataFormat.Binary;
                        var response = Client.ExecutePostAsync(request).Result;

                        var contents = response.Content;

                        var startIndexOf = contents.IndexOf("{\"type\":\"finalResult\"");       // finalResult 시작 index값 구하기

                        if (startIndexOf > 0)
                        {
                                var endIndedxOf = contents.IndexOf("}]") - startIndexOf + 2;              // result FinalResult Model 끝부분의 index - 처음 시작위치 + 2(}] 두글자이기 때문에) 

                                var resultData = JsonSerializer.Deserialize<STT>(contents.Substring(startIndexOf, endIndedxOf) + "}");           // "}" 강제로 model을 닫기위해
                                this.resultAction?.Invoke(new ResultMessage()
                                {
                                        resultCode = Enums.ResultCode.SUCCESS,
                                        message = resultData.value
                                });
                        }
                        else
                        {
                                this.resultAction?.Invoke(new ResultMessage()
                                {
                                        resultCode = Enums.ResultCode.VOICEFAIL,
                                        message = "음성을 인식할 수 없습니다."
                                });
                        }
                }
        }
}
