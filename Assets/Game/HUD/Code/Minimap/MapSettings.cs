using UnityEngine;

using System.IO;

public class MapSettings {

	public string segmentName { get; private set; }

	private int _length, _width;
	private int _xMin, _xMax, _zMin, _zMax;

	public int length { get { return this._length; } }
	public int width { get { return this._width; } }
	
	public int xMin { get { return this._xMin; } }
	public int xMax { get { return this._xMax; } }
	public int zMin { get { return this._zMin; } }
	public int zMax { get { return this._zMax; } }

	public MapSettings(string settingsData) {

		using (var reader = new StringReader(settingsData)) {
			do {
				var line = reader.ReadLine();
				line = line.Replace(" ", string.Empty).ToLower();

				var split = line.Split('=');
				if(split.Length < 1)
					throw new System.IO.FileLoadException("Map data is broken");

				var first = split[0];
				var second = split[1];

				switch (first) {
					case "name":
                {
                    if (second[0] == '\"')
                    {
                        this.segmentName = second.Substring(1, second.Length - 2);
                        if (string.IsNullOrEmpty(segmentName))
                            throw new System.IO.FileLoadException("Map data is broken");
                    }
                    else
                        this.segmentName = second;
					}
					break;
					case "length":
					{
						if(int.TryParse(second, out _length) == false)
							throw new System.IO.FileLoadException("Map data is broken");
					}
					break;
					case "width":
					{
						if(int.TryParse(second, out _width) == false)
							throw new System.IO.FileLoadException("Map data is broken");
					}
					break;
					case "xmin":
					{
						if(int.TryParse(second, out _xMin) == false)
							throw new System.IO.FileLoadException("Map data is broken");
					}
					break;
					case "xmax":
					{
						if(int.TryParse(second, out _xMax) == false)
							throw new System.IO.FileLoadException("Map data is broken");
					}
					break;
					case "zmin":
					{
						if(int.TryParse(second, out _zMin) == false)
							throw new System.IO.FileLoadException("Map data is broken");
					}
					break;
					case "zmax":
					{
						if(int.TryParse(second, out _zMax) == false)
							throw new System.IO.FileLoadException("Map data is broken");
					}
					break;
				}

			} while (reader.Peek() != -1);
		}
	}
}
