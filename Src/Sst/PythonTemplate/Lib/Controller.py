import clr
import sys
clr.AddReference("Ssc")
clr.AddReference("Sss")

from Ssc.Ssc import Controller as PythonController
from Sss.SssScripts.Python import PythonProxy

class Controller(PythonController):
	def __init__(self):
		self.MethodIds = ""

	def Register(self, key,value):
		id = PythonProxy.Register(key,value,sys.PythonHelper)
		if self.MethodIds == "":
			self.MethodIds = id
		else:
			self.MethodIds = self.MethodIds + ";" + id

	def Invoke(self,id,peer,callback,*args):
		PythonProxy.Invoke(id,peer,callback,args)