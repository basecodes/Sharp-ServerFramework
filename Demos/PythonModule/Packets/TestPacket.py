
from ITestPacket import ITestPacket

class TestPacket(ITestPacket):
	def __init__(self):
		ITestPacket.__init__(self)

	def ToBinaryWriter(self,writer):
		writer.Write(self.string,self.Name)
		writer.Write(self.string,self.Password)

	def FromBinaryReader(self,reader):
		self.Name = reader.Read()
		self.Password = reader.Read()