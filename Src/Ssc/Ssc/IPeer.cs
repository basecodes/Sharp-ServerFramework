using System;
using Ssc.SscSerialization;

namespace Ssc.Ssc {
    public interface IPeer :IMessageDispatcher,IRecyclable,IAssignable {
        
        Guid ID { get; }
        Connection Connection { get; }
        ConnectionStatus Status { get; }
        
        void Disconnect();

        void Acknowledge(bool success, ulong messageId);

        void Response(ulong messageId, IResponseMessage responseMessage,IDeserializable singleDeserializable);
    }
}