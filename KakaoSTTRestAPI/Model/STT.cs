using System.Collections.Generic;

namespace KakaoSTTRestAPI.Model
{
        public class STT
        {
                public string type { get; set; }
                public string value { get; set; }
                public List<nBestText> nBest { get; set; }
        }
}
