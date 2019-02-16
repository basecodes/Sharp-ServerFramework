
import clr
clr.AddReference("Ssc")

from Ssc.SscSerialization import SerializablePacket
from ITestPacket import ITestPacket

class TestPacket(SerializablePacket[ITestPacket],ITestPacket):
	def __init__(self):
		ITestPacket.__init__(self)
		self.__TypeName = "ITestPacket"

	@property
	def TypeName(self):
		return self.__TypeName

	@TypeName.setter
	def TypeName(self,value):
		self.__TypeName = value

	def Assign(self):
		pass

	def Recycle(self):
		pass

	def ToBinaryWriter(self,writer):
		writer.Write(self.string,self.Name)
		writer.Write(self.string,self.Password)

	def FromBinaryReader(self,reader):
		self.Name = reader.Read()
		self.Password = reader.Read()