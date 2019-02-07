
from ISerializablePacket import ISerializablePacket

class ITestPacket(ISerializablePacket):
	def __init__(self):
		ISerializablePacket.__init__(self)
		self.Name = ""
		self.Password = ""

	def ToBinaryWriter(self,writer):
		pass

	def FromBinaryReader(self,reader):
		pass

	def ToString(self):
		return self.Name + " " + self.Password