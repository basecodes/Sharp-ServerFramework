
from Entry import Entry
from TestModule import TestModule

class Startup(Entry):
    def __init__(self):
		pass

    def Main(self):
		return TestModule()
