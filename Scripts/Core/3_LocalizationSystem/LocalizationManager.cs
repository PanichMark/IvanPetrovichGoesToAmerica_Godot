using System;
using System.Collections.Generic;
using Godot;

/// <summary>Loads keyed language strings from the localization CSV in game data.</summary>
public sealed class LocalizationManager
{
	private readonly Dictionary<string, Dictionary<LanguagesEnum, string>> _localizations = new(StringComparer.Ordinal);

	public LanguagesEnum CurrentLanguage { get; private set; } = LanguagesEnum.English;

	public delegate void ChangeLanguageEvent(LocalizationManager localizationManager);
	public event ChangeLanguageEvent OnLanguageChanged;

	public LocalizationManager(Bootstrap bootstrap)
	{
		ArgumentNullException.ThrowIfNull(bootstrap);
		LoadFromLocalizationCsv(bootstrap.GameData?.LocalizationMain);
		GD.Print("Localization manager initialized.");
	}

	public void ChangeLanguage(LanguagesEnum language)
	{
		if (!Enum.IsDefined(language))
		{
			GD.PushWarning($"Unsupported language '{language}'. English will be used.");
			language = LanguagesEnum.English;
		}

		CurrentLanguage = language;
		OnLanguageChanged?.Invoke(this);
	}

	public string GetLocalizedString(string key)
	{
		return GetLocalizedString(key, null);
	}

	public string GetLocalizedString(string key, string requestingObjectName)
	{
		if (key != null &&
			_localizations.TryGetValue(key, out Dictionary<LanguagesEnum, string> translations) &&
			translations.TryGetValue(CurrentLanguage, out string translation))
		{
			return translation;
		}

		string requester = string.IsNullOrEmpty(requestingObjectName) ? string.Empty : $" in '{requestingObjectName}'";
		GD.PushWarning($"Localization key '{key}' was not found{requester}.");
		return key ?? string.Empty;
	}

	public string GetNoteLanguageSuffix(InteractionObjectNoteData note)
	{
		if (note == null)
			return string.Empty;

		string path = CurrentLanguage == LanguagesEnum.Russian ? note.NoteText_RU : note.NoteText_EN;
		if (string.IsNullOrWhiteSpace(path))
			path = CurrentLanguage == LanguagesEnum.Russian ? note.NoteText_EN : note.NoteText_RU;
		if (string.IsNullOrWhiteSpace(path))
			return string.Empty;

		if (!FileAccess.FileExists(path))
		{
			GD.PushWarning($"Note text file '{path}' was not found.");
			return string.Empty;
		}

		return FileAccess.GetFileAsString(path);
	}

	private void LoadFromLocalizationCsv(string csvPath)
	{
		if (string.IsNullOrWhiteSpace(csvPath))
		{
			GD.PushWarning("Localization CSV path is empty.");
			return;
		}

		if (!FileAccess.FileExists(csvPath))
		{
			GD.PushWarning($"Localization CSV was not found at '{csvPath}'.");
			return;
		}

		string csv = FileAccess.GetFileAsString(csvPath);
		if (FileAccess.GetOpenError() != Error.Ok)
		{
			GD.PushWarning($"Could not read localization CSV at '{csvPath}'.");
			return;
		}

		List<List<string>> rows = ParseCsv(csv);
		if (rows.Count == 0)
		{
			GD.PushWarning($"Localization CSV at '{csvPath}' is empty.");
			return;
		}

		List<string> headers = rows[0];
		int keyColumn = headers.FindIndex(header => string.Equals(header.Trim(), "Key", StringComparison.OrdinalIgnoreCase));
		int russianColumn = headers.FindIndex(header => string.Equals(header.Trim(), nameof(LanguagesEnum.Russian), StringComparison.OrdinalIgnoreCase));
		int englishColumn = headers.FindIndex(header => string.Equals(header.Trim(), nameof(LanguagesEnum.English), StringComparison.OrdinalIgnoreCase));
		if (keyColumn < 0 || russianColumn < 0 || englishColumn < 0)
		{
			GD.PushWarning($"Localization CSV at '{csvPath}' must have Key, Russian, and English columns.");
			return;
		}

		for (int rowIndex = 1; rowIndex < rows.Count; rowIndex++)
		{
			List<string> row = rows[rowIndex];
			int requiredColumn = Math.Max(keyColumn, Math.Max(russianColumn, englishColumn));
			if (row.Count <= requiredColumn)
			{
				if (row.Count == 1 && string.IsNullOrWhiteSpace(row[0]))
				{
					continue;
				}

				GD.PushWarning($"Skipping incomplete localization CSV row {rowIndex + 1}.");
				continue;
			}

			string key = row[keyColumn].Trim();
			if (key.Length == 0)
			{
				continue;
			}

			_localizations[key] = new Dictionary<LanguagesEnum, string>
			{
				[LanguagesEnum.Russian] = row[russianColumn],
				[LanguagesEnum.English] = row[englishColumn]
			};
		}
	}

	private static List<List<string>> ParseCsv(string csv)
	{
		List<List<string>> rows = new();
		List<string> row = new();
		System.Text.StringBuilder field = new();
		bool insideQuotedField = false;

		for (int index = 0; index < csv.Length; index++)
		{
			char character = csv[index];
			if (character == '"')
			{
				if (insideQuotedField && index + 1 < csv.Length && csv[index + 1] == '"')
				{
					field.Append('"');
					index++;
				}
				else
				{
					insideQuotedField = !insideQuotedField;
				}
			}
			else if (!insideQuotedField && character == ',')
			{
				row.Add(field.ToString());
				field.Clear();
			}
			else if (!insideQuotedField && (character == '\r' || character == '\n'))
			{
				row.Add(field.ToString());
				field.Clear();
				rows.Add(row);
				row = new List<string>();
				if (character == '\r' && index + 1 < csv.Length && csv[index + 1] == '\n')
				{
					index++;
				}
			}
			else
			{
				field.Append(character);
			}
		}

		if (field.Length > 0 || row.Count > 0)
		{
			row.Add(field.ToString());
			rows.Add(row);
		}

		if (rows.Count > 0 && rows[0].Count > 0)
		{
			rows[0][0] = rows[0][0].TrimStart('\uFEFF');
		}

		return rows;
	}
}