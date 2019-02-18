
from SerializablePacket import SerializablePacket

class ITestPacket(SerializablePacket):
	def __init__(self):
		SerializablePacket.__init__(self,"ITestPacket")
		self.Name = ""
		self.Password = ""

	def ToBinaryWriter(self,writer):
		pass

	def FromBinaryReader(self,reader):
		pass

	def ToString(self):
		return self.Name + " " + self.Password