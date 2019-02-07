using Ssc.Ssc;
using Ssc.SscStream;

namespace Ssf.Ssf {
    #region Internal

    internal delegate bool ProxyInvoke(string className, IWriteStream writeStream, string methodName,
        IPeer player, ResponseCallback responseCallback);

    internal delegate void Connected(IPeer peer, IReadStream readStream,IWriteStream writeStream);

    #endregion

    #region Public

    public delegate void Encryptor(IWriteStream writeStream, string sharedSecret);

    public delegate void Decryptor(IReadStream writeStream, string sharedSecret);

    #endregion
}