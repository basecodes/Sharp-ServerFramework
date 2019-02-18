import clr
import sys

clr.AddReference("Sss")
clr.AddReference("System.Runtime")

from System import TypeCode
from Sss.SssSerialization.Python import PythonPacket
from Sss.SssScripts.Python import PythonProxy

class SerializablePacket:
	def __init__(self,interface):
		self.ISerializablePacket = PythonProxy.CreatePacket(interface,self,sys.PythonHelper)

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

	def Assign(self):
		pass

	def Recycle(self):
		pass

	def ToBinaryWriter(self,writer):
		pass

	def FromBinaryReader(self,reader):
		pass