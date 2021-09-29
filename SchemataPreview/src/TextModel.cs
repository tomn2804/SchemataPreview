﻿using System.IO;

namespace SchemataPreview
{
	public class TextModel : FileModel
	{
		public TextModel(ImmutableSchema schema)
			: base(schema)
		{
		}

		public string[] Contents
		{
			get => File.ReadAllLines(FullName);
			set => File.WriteAllLines(FullName, value);
		}
	}
}
