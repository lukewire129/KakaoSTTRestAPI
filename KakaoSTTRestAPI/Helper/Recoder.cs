using NAudio.Wave;
using KakaoSTTRestAPI.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KakaoSTTRestAPI.Helper
{
        public class Recorder: baseRecoder, IRecorder
        {

                WaveIn sourceStream;
                WaveFileWriter waveWriter;
                readonly String FilePath;
                readonly int InputDeviceIndex;

                public Recorder(int inputDeviceIndex, String filePath)
                {
                        this.InputDeviceIndex = inputDeviceIndex;
                        this.FilePath = filePath;
                }

                public override void StartRecording()
                {
                        sourceStream = new WaveIn
                        {
                                DeviceNumber = this.InputDeviceIndex,
                                WaveFormat =
                                new WaveFormat(16000, 1)
                        };

                        sourceStream.DataAvailable += this.SourceStreamDataAvailable;

                        if (!Directory.Exists(FilePath))
                        {
                                Directory.CreateDirectory(FilePath);
                        }

                        waveWriter = new WaveFileWriter(FilePath + this.fileName, sourceStream.WaveFormat);
                        sourceStream.StartRecording();
                }

                public void SourceStreamDataAvailable(object sender, WaveInEventArgs e)
                {
                        if (waveWriter == null) return;
                        waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                        waveWriter.Flush();
                }

                public override void RecordEnd()
                {
                        if (sourceStream != null)
                        {
                                sourceStream.StopRecording();
                                sourceStream.Dispose();
                                sourceStream = null;
                        }
                        if (this.waveWriter == null)
                        {
                                return;
                        }
                        this.waveWriter.Dispose();
                        this.waveWriter = null;

                        this.data = File.ReadAllBytes($"{FilePath}{ this.fileName}");

                        Task.Run(() =>
                        {
                                GetResultText();
                        });
                }

                public override IRecorder SetApiKey(string _apiKey)
                {
                        this.apiKey = _apiKey;
                        return this;
                }
        }
}
