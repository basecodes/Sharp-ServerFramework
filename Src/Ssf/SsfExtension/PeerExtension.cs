namespace Ssf.SsfExtension {
//     public static class PeerExtension {
// 
//         public static void Send(this IPeer peer, ushort opCode, ISingleSerializable singleSerializable) {
//             Send(peer, opCode, singleSerializable, null);
//         }
// 
//         public static void Send(this IPeer peer, ushort opCode,
//             ISingleSerializable singleSerializable, ResponseCallback responseCallback) {
//             var message = new RawMessage(opCode, null);
//             if (singleSerializable is SingleSerializable serializable) {
//                 message.SerializeFields(serializable.WriteStream);
//                 peer.SendMessage(serializable.WriteStream,responseCallback);
//             }
//         }
// 
//         public static void Invoke(this IPeer peer, Serializable serializable) {
//             Invoke(peer, serializable, null);
//         }
// 
//         public static void Invoke(this IPeer peer,Serializable serializable, ResponseCallback responseCallback) {
//             RpcProxy.Invoke(serializable.WriteStream,serializable.GetMethodID(), peer, responseCallback);
//         }
//     }
}