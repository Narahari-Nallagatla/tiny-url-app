export interface TinyUrl {
  code: string;
  shortURL: string;    // Matches "shortURL" in your JSON
  originalURL: string; // Matches "originalURL" in your JSON
  totalClicks: number; // Matches "totalClicks" in your JSON
  isPrivate: boolean;
}

export interface TinyUrlAddDto {
  OriginalURL: string; // Keep PascalCase as per your previous successful POST test
  IsPrivate: boolean;
}