﻿
using System.Collections.Generic;

namespace uw_edit.USFM
{
	public class TagError
	{
		public int LineNumber { get; private set; }
		public int CharacterIndex { get; private set; }
		public string HintText { get; private set; }

		public TagError(int lineNumber, int characterIndex, string hintText)
		{
			LineNumber = lineNumber;
			CharacterIndex = characterIndex;
			HintText = hintText;
		}
	}

	public class TagErrors
	{
		public int BeginLineNumber { get; private set; }
		public int EndLineNumber { get; private set; }
		public List<TagError> Errors { get; private set; }

		public TagErrors()
		{
			Errors = new List<TagError>();
			BeginLineNumber = int.MaxValue;
			EndLineNumber = 0;
		}

		public void Add(TagError error)
		{
			Errors.Add(error);
			if (error.LineNumber < BeginLineNumber)
				BeginLineNumber = error.LineNumber;

			if (error.LineNumber > EndLineNumber)
				EndLineNumber = error.LineNumber;
		}
	}
}