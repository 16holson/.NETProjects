using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Shared {
    public class Assets {

        public string? AssetFile { get; set; }

        public Assets(string? assetFile) {
            this.AssetFile = assetFile;
        }
    }
}
