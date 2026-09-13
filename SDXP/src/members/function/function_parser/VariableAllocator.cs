namespace SDSL;

public class VariableAllocator
{
	// Useful for error locations
	private readonly TokenStream _stream;
	
	// Maps all the variables currently defined to their stack location
	private readonly Dictionary<string, int> _variableMap = [];
	
	// A stack containing the sets of the variable names defined in the current scope
	private readonly Stack<List<string>> _scopes = [];
	
	// All the locations which are currently not in use
	private readonly Stack<int> _freeLocations = [];

	private int _variableCount;

	public VariableAllocator(TokenStream stream)
	{
		_stream = stream;
	}

	public int VariableCount => _variableCount;

	public void OpenScope()
	{
		_scopes.Push([]);
	}

	public void CloseScope()
	{
		if (!_scopes.TryPop(out List<string> variableNames))
		{
			throw new ParserException(_stream,
				"No scopes have been open.");
		}

		for (int i = 0; i < variableNames.Count; i++)
		{
			string name = variableNames[i];
            
			if (!_variableMap.Remove(name, out int location))
			{
				throw new ParserException(_stream,
					$"Failed to delete variable '{name}'.");
			}
            
			_freeLocations.Push(location);
		}
	}
	
	public int DefineVariable(string name)
	{
		if (_variableMap.ContainsKey(name))
		{
			throw new ParserException(_stream,
				$"Variable '{name}' has already been defined.");
		}
        
		if (!_scopes.TryPeek(out List<string> variableNames))
		{
			throw new ParserException(_stream,
				"No scopes have been defined.");
		}
        
		variableNames.Add(name);

		if (!_freeLocations.TryPop(out int location))
		{
			location = _variableCount++;
		}

		_variableMap.Add(name, location);

		return location;
	}
	
	public bool TryGetVariableLocation(string name, out int location)
	{
		return _variableMap.TryGetValue(name, out location);
	}
}