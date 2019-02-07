local Classes = require "Classes"

local Loader = Classes.class()

function Loader:init(fileName)
	self.Assembly = LuaLoader:LoadAssembly(fileName)
end

function Loader:GetType(name)
	return LuaLoader:GetType(self.Assembly,name)
end


return Loader