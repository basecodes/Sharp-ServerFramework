using System.Threading;

namespace Ssf.SsfNetwork.Sockets {
    public class SocketStatistics {
        public long MessagesSent =>
            UnreliableMessagesSent +
            ReliableMessagesSent +
            FragmentedMessagesSent +
            AcknowledgementMessagesSent +
            AckMessagesSent;

        public long MessagesReceived =>
            UnreliableMessagesReceived +
            ReliableMessagesReceived +
            FragmentedMessagesReceived +
            AcknowledgementMessagesReceived +
            ackMessagesReceived;

        #region 发送的不可靠消息的数量。

        private long unreliableMessagesSent;

        public long UnreliableMessagesSent => Interlocked.Read(ref unreliableMessagesSent);

        #endregion 发送的不可靠消息的数量。

        #region 发送的可靠消息的数量。

        private long reliableMessagesSent;

        public long ReliableMessagesSent => Interlocked.Read(ref reliableMessagesSent);

        #endregion 发送的可靠消息的数量。

        #region 发送的分片消息的数量。

        private long fragmentedMessagesSent;

        public long FragmentedMessagesSent => Interlocked.Read(ref fragmentedMessagesSent);

        #endregion 发送的分片消息的数量。

        #region 发送的确认消息的数量。

        private long acknowledgementMessagesSent;

        public long AcknowledgementMessagesSent => Interlocked.Read(ref acknowledgementMessagesSent);

        #endregion 发送的确认消息的数量。

        #region 发送的Ack消息的数量。

        public long AckMessagesSent => Interlocked.Read(ref ackMessagesSent);

        private long ackMessagesSent;

        #endregion 发送的hello消息的数量。

        #region 发送的数据的字节数。

        private long dataBytesSent;

        public long DataBytesSent => Interlocked.Read(ref dataBytesSent);

        #endregion 发送的数据的字节数。

        #region 总共发送的字节数。

        private long totalBytesSent;

        public long TotalBytesSent => Interlocked.Read(ref totalBytesSent);

        #endregion 总共发送的字节数。

        #region 收到的不可靠消息的数量。

        private long unreliableMessagesReceived;

        public long UnreliableMessagesReceived => Interlocked.Read(ref unreliableMessagesReceived);

        #endregion 收到的不可靠消息的数量。

        #region 收到的可靠消息的数量。

        private long reliableMessagesReceived;

        public long ReliableMessagesReceived => Interlocked.Read(ref reliableMessagesReceived);

        #endregion 收到的可靠消息的数量。

        #region 收到的分片消息的数量。

        private long fragmentedMessagesReceived;

        public long FragmentedMessagesReceived => Interlocked.Read(ref fragmentedMessagesReceived);

        #endregion 收到的分片消息的数量。

        #region 接收到的确认消息的数量。

        private long acknowledgementMessagesReceived;

        public long AcknowledgementMessagesReceived => Interlocked.Read(ref acknowledgementMessagesReceived);

        #endregion 接收到的确认消息的数量。

        #region 收到的Ack消息的数量。

        private long ackMessagesReceived;

        public long AckMessagesReceived => Interlocked.Read(ref ackMessagesReceived);

        #endregion 收到的hello消息的数量。

        #region 接收的数据的字节数。

        private long dataBytesReceived;

        public long DataBytesReceived => Interlocked.Read(ref dataBytesReceived);

        #endregion 接收的数据的字节数。

        #region 总共收到的字节数。

        private long totalBytesReceived;

        public long TotalBytesReceived => Interlocked.Read(ref totalBytesReceived);

        #endregion 总共收到的字节数。

        #region Send

        public void LogDataBytesSent(int dataLength) {
            Interlocked.Add(ref dataBytesSent, dataLength);
        }

        public void LogTotalBytesSent(int totalLength) {
            Interlocked.Add(ref totalBytesSent, totalLength);
        }

        public void LogUnreliableSend() {
            Interlocked.Increment(ref unreliableMessagesSent);
        }

        public void LogReliableSend() {
            Interlocked.Increment(ref reliableMessagesSent);
        }

        public void LogFragmentedSend() {
            Interlocked.Increment(ref fragmentedMessagesSent);
        }

        public void LogAcknowledgementSend() {
            Interlocked.Increment(ref acknowledgementMessagesSent);
        }

        public void LogAckSend() {
            Interlocked.Increment(ref ackMessagesSent);
        }

        #endregion

        #region Receive

        public void LogDataBytesReceived(int dataLength) {
            Interlocked.Add(ref dataBytesReceived, dataLength);
        }

        public void LogTotalBytesReceived(int totalLength) {
            Interlocked.Add(ref totalBytesReceived, totalLength);
        }

        public void LogUnreliableReceive() {
            Interlocked.Increment(ref unreliableMessagesReceived);
        }

        public void LogReliableReceive() {
            Interlocked.Increment(ref reliableMessagesReceived);
        }

        public void LogFragmentedReceive() {
            Interlocked.Increment(ref fragmentedMessagesReceived);
        }

        public void LogAcknowledgementReceive() {
            Interlocked.Increment(ref acknowledgementMessagesReceived);
        }

        public void LogAckReceive() {
            Interlocked.Increment(ref ackMessagesReceived);
        }

        #endregion
    }
}