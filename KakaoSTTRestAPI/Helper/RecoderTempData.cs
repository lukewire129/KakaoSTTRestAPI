using NAudio.Wave;
using KakaoSTTRestAPI.Interfaces;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KakaoSTTRestAPI.Helper
{
        public class RecoderTempData: baseRecoder, IRecorder
        {
                readonly int InputDeviceIndex;
                private WaveIn sourceStream;
                private BinaryWriter binaryWriter;
                private MemoryStream ms;
                public RecoderTempData(int inputDeviceIndex)
                {
                        this.InputDeviceIndex = inputDeviceIndex;
                }

                private void CreateFactChunk()
                {
                        if (HasFactChunk())
                        {
                                binaryWriter.Write(Encoding.UTF8.GetBytes("fact"));
                                binaryWriter.Write(4);
                                binaryWriter.Write(0);
                        }
                }

                private bool HasFactChunk()
                {
                        if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
                        {
                                return sourceStream.WaveFormat.BitsPerSample != 0;
                        }

                        return false;
                }
                private void WriteDataChunkHeader()
                {
                        binaryWriter.Write(Encoding.UTF8.GetBytes("data"));
                        binaryWriter.Write(0);
                }

                public void SourceStreamDataAvailable(object sender, WaveInEventArgs e)
                {
                        if (binaryWriter == null) return;
                        binaryWriter.Write(e.Buffer, 0, e.BytesRecorded);
                        binaryWriter.Flush();
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

                        ms = new MemoryStream();

                        binaryWriter = new BinaryWriter(ms, Encoding.UTF8);
                        binaryWriter.Write(Encoding.UTF8.GetBytes("RIFF"));
                        binaryWriter.Write(0);
                        binaryWriter.Write(Encoding.UTF8.GetBytes("WAVE"));
                        binaryWriter.Write(Encoding.UTF8.GetBytes("fmt "));

                        sourceStream.WaveFormat.Serialize(binaryWriter);
                        CreateFactChunk();
                        WriteDataChunkHeader();

                        sourceStream.StartRecording();
                }

                public override void RecordEnd()
                {
                        if (sourceStream != null)
                        {
                                sourceStream.StopRecording();
                                sourceStream.Dispose();
                                sourceStream = null;
                        }
                        if (this.binaryWriter == null)
                        {
                                return;
                        }
                        this.binaryWriter.Dispose();
                        this.binaryWriter = null;

                        this.data = ms.ToArray();

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
