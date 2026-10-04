using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

/// <summary>Reads and writes save data beneath the Godot user data directory.</summary>
public sealed class JsonFileDataHandler
{
	private readonly string _dataDirectory;
	private readonly string _dataFileName;
	private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

	public JsonFileDataHandler(string dataDirPath, string dataFileName)
	{
		_dataDirectory = dataDirPath ?? throw new ArgumentNullException(nameof(dataDirPath));
		_dataFileName = dataFileName ?? throw new ArgumentNullException(nameof(dataFileName));
	}

	public JsonGameData Load() => LoadFromFile(_dataFileName);

	public JsonGameData LoadFromFile(string fileName)
	{
		string fullPath = Path.Combine(_dataDirectory, fileName);
		if (!File.Exists(fullPath))
			return null;

		try
		{
			return JsonSerializer.Deserialize<JsonGameData>(File.ReadAllText(fullPath), SerializerOptions);
		}
		catch (Exception exception)
		{
			GD.PushError($"Could not load save file '{fullPath}': {exception}");
			return null;
		}
	}

	public void Save(JsonGameData data)
	{
		if (data == null)
			throw new ArgumentNullException(nameof(data));

		string fullPath = Path.Combine(_dataDirectory, _dataFileName);
		try
		{
			Directory.CreateDirectory(_dataDirectory);
			File.WriteAllText(fullPath, JsonSerializer.Serialize(data, SerializerOptions));
		}
		catch (Exception exception)
		{
			GD.PushError($"Could not save file '{fullPath}': {exception}");
			throw;
		}
	}

	private static JsonSerializerOptions CreateSerializerOptions()
	{
		var options = new JsonSerializerOptions
		{
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNameCaseInsensitive = true
		};
		options.Converters.Add(new JsonStringEnumConverter());
		options.Converters.Add(new Vector3Converter());
		options.Converters.Add(new QuaternionConverter());
		return options;
	}

	public sealed class Vector3Converter : JsonConverter<Vector3>
	{
		public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using JsonDocument document = JsonDocument.ParseValue(ref reader);
			JsonElement value = document.RootElement;
			return new Vector3(ReadFloat(value, "x"), ReadFloat(value, "y"), ReadFloat(value, "z"));
		}

		public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("x", value.X);
			writer.WriteNumber("y", value.Y);
			writer.WriteNumber("z", value.Z);
			writer.WriteEndObject();
		}
	}

	public sealed class QuaternionConverter : JsonConverter<Quaternion>
	{
		public override Quaternion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using JsonDocument document = JsonDocument.ParseValue(ref reader);
			JsonElement value = document.RootElement;
			return new Quaternion(ReadFloat(value, "x"), ReadFloat(value, "y"), ReadFloat(value, "z"), ReadFloat(value, "w"));
		}

		public override void Write(Utf8JsonWriter writer, Quaternion value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("x", value.X);
			writer.WriteNumber("y", value.Y);
			writer.WriteNumber("z", value.Z);
			writer.WriteNumber("w", value.W);
			writer.WriteEndObject();
		}
	}

	private static float ReadFloat(JsonElement element, string name)
	{
		if (element.TryGetProperty(name, out JsonElement value) && value.TryGetSingle(out float result))
			return result;
		return 0f;
	}
}