using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Manning.MyPhotoAlbum
{
    public class AlbumStorageExpection : Exception
    {
        public AlbumStorageExpection() : base() { }
        public AlbumStorageExpection(string msg) : base(msg) { }
        public AlbumStorageExpection(string msg, Exception inner) : base(msg, inner) { }
    }

    public class AlbumStorage
    {
        static private int CurrentVersion = 63;

        static public void WriteAlbum(PhotoAlbum album, string path)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(path, false);
                sw.WriteLine(CurrentVersion.ToString());

                foreach (Photograph p in album)
                    WritePhoto(sw, p);

                album.HasChaged = false;
            }
            catch (UnauthorizedAccessException uax)
            {
                throw new AlbumStorageExpection("Unable to access album " + path, uax);  
            }
             finally
            {
                 if (sw != null)
                     sw.Close();

            }
        }
        
        static private void WritePhoto(StreamWriter sw, Photograph p)
        {
            sw.WriteLine(p.FileName);

            sw.WriteLine(p.Caption != null ? p.Caption : "");
            sw.WriteLine(p.DateTaken.ToString());
            sw.WriteLine(p.Photographer != null ? p.Photographer : "");
            sw.WriteLine(p.Notes != null ? p.Notes : "");
        }

         static public PhotoAlbum ReadAlbum(string path)
         {
         StreamReader sr = null;

         try
         {
              sr = new StreamReader(path);
              string Version = sr.ReadLine();

              PhotoAlbum album = new PhotoAlbum();
              switch (Version)
              {
                    case "63":
                      ReadAlbumV63(sr, album);
                      break;
                  default:
                      throw new AlbumStorageExpection("Unrecognizedalbum version");
              }

              album.HasChaged = false;
              return album;
         }
         catch (FileNotFoundException fnx)
         {
             throw new AlbumStorageExpection("Unable to read album " + path, fnx);
         }
         finally
         {
             if (sr != null)
                 sr.Close();
         }
       }

          static private void ReadAlbumV63(StreamReader sr, PhotoAlbum album)
          {
              Photograph p;
              do
              {
                  p = ReadPhotoV63(sr);
                  if (p != null)
                      album.Add(p);
              } while (p != null);     
          } 
          static private Photograph ReadPhotoV63(StreamReader sr )
          {
               string file = sr.ReadLine();
               if (file == null || file.Length == 0)
                   return null;

               Photograph p = new Photograph(file);

               p.Caption = sr.ReadLine();
               p.DateTaken = DateTime.Parse(sr.ReadLine());
               p.Photographer = sr.ReadLine();
               p.Notes = sr.ReadLine();

               return p;
          }
      }
  }