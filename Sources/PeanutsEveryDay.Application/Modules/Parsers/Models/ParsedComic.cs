﻿using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Application.Modules.Parsers.Models;

public record ParsedComic(
    DateOnly PublicationDate,
    SourceType Source,
    string Url,
    string ImagePath,
    Stream ImageStream);