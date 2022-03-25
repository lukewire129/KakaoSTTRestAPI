using KakaoSTTRestAPI.Helper;
using KakaoSTTRestAPI.Interfaces;
using KakaoSTTRestAPI.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace KakaoSTTRestAPI
{
        /// <summary>
        /// MainWindow.xaml에 대한 상호 작용 논리
        /// </summary>
        public partial class MainWindow : Window
        {
                private IRecorder recorder;
                public MainWindow()
                {
                        InitializeComponent();
                }

                private void ResultAction(ResultMessage resultMessage)
                {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                                switch (resultMessage.resultCode)
                                {
                                        case Enums.ResultCode.SUCCESS:
                                                this.lbResult.Content = resultMessage.message;
                                                break;
                                        case Enums.ResultCode.VOICEMEMORYDATAISNULL:
                                                this.lbResult.Content = "Voice 데이터가 비어있습니다.";
                                                break;
                                        case Enums.ResultCode.APIKEYISNULL:
                                                this.lbResult.Content = "API key값이 비어있습니다.";
                                                break;
                                        case Enums.ResultCode.VOICEFAIL:
                                                this.lbResult.Content = resultMessage.message;
                                                break;
                                };
                        }));
                }
                private void btnVoice_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        lbResult.Content = "";
                        recorder = new Recorder(0, "./voice/")
                                                .SetApiKey(apiKey.Text);
                        recorder.resultAction += ResultAction;
                        recorder.StartRecording();
                }

                private void  btnVoice_MouseUp(object sender, MouseButtonEventArgs e)
                {
                        recorder.RecordEnd();
                }

                private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
                {
                        lbResult.Content = "";
                        recorder = new RecoderTempData(0)
                                        .SetApiKey(apiKey.Text);
                        recorder.resultAction += ResultAction;
                        recorder.StartRecording();
                }

                private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
                {
                        recorder.RecordEnd();
                }
        }
}
