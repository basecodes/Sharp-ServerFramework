import clr
clr.AddReference("Ssc")
clr.AddReference("System.Runtime")

from Ssc.SscSerialization import ISerializablePacket as PythonISerializablePacket
from System import TypeCode

class ISerializablePacket(PythonISerializablePacket):
	def __init__(self):
		PythonISerializablePacket.__init__(self)
		self.char = TypeCode.Char
		self.bool = TypeCode.Boolean
		self.sbyte = TypeCode.SByte
		self.byte = TypeCode.Byte
		self.short = TypeCode.Int16
		self.ushort = TypeCode.UInt16
		self.int = TypeCode.Int32
		self.uint = TypeCode.UInt32
		self.long = TypeCode.Int64
		self.ulong = TypeCode.UInt64
		self.float = TypeCode.Single
		self.double = TypeCode.Double
		self.string = TypeCode.String

	def ToBinaryWriter(self,writer):
		PythonISerializablePacket.ToBinaryWriter(self,writer)

	def FromBinaryReader(self,reader):
		PythonISerializablePacket.FromBinaryReader(self,reader)