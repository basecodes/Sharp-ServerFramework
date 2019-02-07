local Classes = require "Classes"
local Entry = require "Entry"
local TestModule = require "TestModule"

local startup = Classes:class(Entry)

function startup:Main()
	return TestModule.new()
end

return startup