

using UnityEngine;

namespace NextFrameworkForYao.Res
{
    public class BytesAsset : TextAsset
    {
        private byte[] _bytes;

        public byte[] Bytes => this._bytes;

        public BytesAsset(byte[] bytes) => this._bytes = bytes;
    }
}
