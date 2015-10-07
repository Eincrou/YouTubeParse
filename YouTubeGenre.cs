using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeParse
{
    /// <summary>
    /// Genres (Categories) of YouTube videos
    /// </summary>
    public enum YouTubeGenre
    {
        AutosAndVehicles, Comedy, Education, Entertainment, FilmAndAnimation,
        Gaming, HowtoAndStyle, Music, NewAndPolitics, NonprofitsAndActivism,
        PeopleAndBlogs, PetsAndAnimals, ScienceAndTechnology, Sports, TravelAndEvents,
        Unknown
    }

    public static class YouTubeGenreHelper
    {
        public static YouTubeGenre StringToEnum(string genreStr)
        {
            YouTubeGenre ytg;
            if(!Enum.TryParse(genreStr, true, out ytg))
            {
                switch (genreStr)
                {
                    case "Autos & Vehicles":
                        ytg = YouTubeGenre.AutosAndVehicles;
                        break;
                    case "Film & Animation":
                        ytg = YouTubeGenre.FilmAndAnimation;
                        break;
                    case "Howto & Style":
                        ytg = YouTubeGenre.HowtoAndStyle;
                        break;
                    case "News & Politics":
                        ytg = YouTubeGenre.NewAndPolitics;
                        break;
                    case "Nonprofits & Activism":
                        ytg = YouTubeGenre.NonprofitsAndActivism;
                        break;
                    case "People & Blogs":
                        ytg = YouTubeGenre.PeopleAndBlogs;
                        break;
                    case "Pets & Animals":
                        ytg = YouTubeGenre.PetsAndAnimals;
                        break;
                    case "Science & Technology":
                        ytg = YouTubeGenre.ScienceAndTechnology;
                        break;
                    case "Travel & Events":
                        ytg = YouTubeGenre.TravelAndEvents;
                        break;
                    default:
                        ytg = YouTubeGenre.Unknown;
                        break;
                }
            }
            return ytg;
        }
    }
}