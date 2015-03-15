using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using System.IO;
using System.Diagnostics;

namespace lifedungeon
{
    public class Map
    {
        Random rand { get; set; }
        Point dimensions { get; set; }
        List<Tuple<Point, Point>> types { get; set; }

        int[,] mapArray { get; set; }

        List<Room> rooms { get; set; }
        int roomLimit;

        public Map(Random r, Point v, List<Tuple<Point, Point>> t)
        {
            rand = r;
            dimensions = v;
            types = t;
            mapArray = new int[dimensions.X, dimensions.Y];
            roomLimit = rand.Next(128,164);
            rooms = new List<Room>();
        }

        //public void buildRooms(){
        //    //Phase 1 - Center room

        //    //Point p = new Point(dimensions.X / 2, dimensions.Y / 2); // Find the center point.
            
        //    // Choose room type.
        //    int t = rand.Next(0, types.Count - 1);

        //    // Get lower & upper limits.
        //    Tuple<Point,Point> bbox = types[t]; //Small, med, large.

        //    // Choose a size between lower & upper.
        //    Point size = new Point( rand.Next( bbox.Item1.X, bbox.Item2.X ), rand.Next( bbox.Item1.Y, bbox.Item2.Y ));
        //    Point p = new Point(rand.Next(0, dimensions.X - size.X), rand.Next(0, dimensions.Y-size.Y));
        //    Debug.WriteLine("\nCenter point selected, i: {0}, j:{1}\n", p.X, p.Y);

        //    Point o = new Point( p.X - (size.X/2), p.Y - (size.Y/2));
        //    digRoom( o, t, size);

        //    // Pick spawn point ( this will be -1 )
        //    mapArray[ rand.Next( o.X + 1, o.X + size.X - 1), rand.Next( o.Y + 1, o.Y + size.Y -1 )] = -1;

        //    //Pick a direction for the wall to build in.
        //    Point pick = new Point(0, 0);
        //    int dir = pickDir();

        //    // Get some walls.
        //    getWallsFromDir(dir, o, size);
            


        //}

        public Room generateRoom()
        {
            // Choose a type
            int t = rand.Next(0, types.Count - 1);

            // Get lower & upper limits.
            Tuple<Point,Point> bbox = types[t]; //Small, med, large.

            // Choose a size between lower & upper.
            Point size;
            do
            {
                size = new Point(rand.Next(bbox.Item1.X, bbox.Item2.X), rand.Next(bbox.Item1.Y, bbox.Item2.Y));
            } while (size.X % 2 == 0 || size.Y % 2 == 0);
            Console.WriteLine("{0},{1}",size.X, size.Y);

            //Point size = new Point(19, 19);
            Point p = new Point(rand.Next(0, dimensions.X - size.X), rand.Next(0, dimensions.Y - size.Y));
            //Point p = new Point( dimensions.X / 2, dimensions.Y /2 );
            //Point o = new Point(p.X - (size.X / 2), p.Y - (size.Y / 2));
            return new Room(p, size, t);
        }

        public void buildRooms()
        {
            // Phase 1 - Random Rooms
            do
            {
                Room r = generateRoom();

                while (!isFree(r.pos, r.type, r.size))
                {
                    r = generateRoom();
                }
                // Must be successful
                rooms.Add(r);
                writeRoom(r);
            } while (rooms.Count < roomLimit);

            outputMap();
        }

        public void digRoom(Point origin, int type, Point size )
        {
            for( int i = origin.X; i < origin.X + size.X; ++i ){
                for (int j = origin.Y; j < origin.Y + size.Y; ++j)
                {

                    // Walls around perim
                    int offx = i - origin.X;
                    int offy = j - origin.Y;
                    if (offx == 0 || offx == size.X - 1 || offy == 0 || offy == size.Y - 1)
                    {
                        Debug.WriteLine("Walls placed, i: {0}, j: {1}", i, j);
                        mapArray[i, j] = 1; // Walls!
                    }
                    else
                    {
                        mapArray[i, j] = 2; // Floor
                    }

                    
                }
            }
        
        }

        public bool isFree(Point origin, int type, Point size)
        {
            // Check within bounds
            if (!withinBounds(origin, size)) return false;

            // Check for room overlap
            for (int j = origin.X - size.X/2; j <= origin.X + size.X/2; ++j)
            {
                for (int i = origin.Y - size.Y/2; i <= origin.Y + size.Y/2; ++i)
                {
                    if (mapArray[i-1, j-1] != 0)
                    {
                        return false;
                    }
                    else
                    {
                        System.Console.WriteLine(mapArray[i - 1, j - 1]);
                    }
                }
            }

            return true;
        }

        public bool withinBounds( Point origin, Point size )
        {
            return (origin.X - size.X / 2 > 0 && origin.X + size.X /2 < dimensions.X) && (origin.Y - size.Y/2 > 0 && origin.Y + size.Y/2 < dimensions.Y);
        }

        //public int pickDir()
        //{
        //    return rand.Next(0, 3);
        //}

        //public List<Point> getWallsFromDir( int d, Point o, Point s )
        //{
        //    List<Point> rtn = new List<Point>();
        //    for (int i = o.X; i < o.X + s.X; ++i)
        //    {
        //        for (int j = o.Y; j < o.Y + s.Y; ++j)
        //        {
        //            //switch (d)
        //            //{
        //            //    case 0:
        //            //        //Up
        //            //        if (i > o.X && i < o.X + s.X && j == o.Y)
        //            //        {
        //            //            rtn.Add(new Point(i, j));
        //            //        }
        //            //        break;
        //            //    case 1:
        //            //        //Left
        //            //        if (i == o.X && j > o.Y + 1 && j < o.Y + s.Y)
        //            //        {
        //            //            rtn.Add(new Point(i, j));
        //            //        }
        //            //        break;
        //            //    case 2:
        //            //        //Down
        //            //        if (i > o.X && i < o.X + s.X && j == o.Y + s.Y)
        //            //        {
        //            //            rtn.Add(new Point(i, j));
        //            //        }
        //            //        break;
        //            //    case 3:
        //            //        //Right
        //            //        if (i == o.X + s.X - 1)
        //            //            break;
        //            //}

        //            //Corner exclusion
        //            if ((i == o.X && j == o.Y) || (i == o.X + s.X && j == o.Y) || (i == o.X && j == o.Y + s.Y) || (i == o.X + s.X && j == o.Y + s.Y))
        //            {
        //                //CORNERS BE SKIPPED
        //                continue;
        //            }

        //            //Main body exclusion
        //            else if ((i > o.X && i < o.X + s.X) && (j > o.Y && j < o.Y + s.Y))
        //            {
        //                // MAIN BODY MUST BE EXCLUDED
        //                continue;
        //            }
        //            else
        //            {
        //                // SUCCESS ADD OUR WALLS
        //                rtn.Add(new Point(i, j));
        //            }


        //        }
        //    }
        //    return rtn;
        //}

        //public bool canPlace(Point origin, int type)
        //{
        //    return isFree(origin, type, types[type].Item1);
        //} 

        public void writeRoom(Room r)
        {
            for (int j = r.pos.X - r.size.X / 2; j <= r.pos.X + r.size.X / 2; ++j)
            {
                for (int i = r.pos.Y - r.size.Y / 2; i <= r.pos.Y + r.size.Y / 2; ++i)
                {
                    // Draw the floor
                    mapArray[i - 1, j - 1] = 2;

                    int xoff = j - (r.pos.X - (r.size.X / 2));
                    int yoff = i - (r.pos.Y - (r.size.Y / 2));

                    if (xoff == 0 || xoff == r.size.X - 1 || yoff == 0 || yoff == r.size.Y - 1)
                    {
                        // Should be a wall, draw 1's.
                        mapArray[i - 1, j - 1] = 1;
                    }
                }
            }
        }

        //public void writeMap()
        //{
        //    foreach (Room r in this.rooms)
        //    {
        //        for (int j = r.pos.X - r.size.X / 2; j <= r.pos.X + r.size.X / 2; ++j)
        //        {
        //            for (int i = r.pos.Y - r.size.Y / 2; i <= r.pos.Y + r.size.Y / 2; ++i)
        //            {
        //                // Draw the floor
        //                mapArray[i-1, j-1] = 2;

        //                int xoff = j - (r.pos.X - (r.size.X / 2));
        //                int yoff = i - (r.pos.Y - (r.size.Y / 2));
                        
        //                if (xoff == 0 || xoff == r.size.X - 1  || yoff == 0 || yoff == r.size.Y - 1)
        //                {
        //                    // Should be a wall, draw 1's.
        //                    mapArray[i-1, j-1] = 1;
        //                }
        //            }
        //        }
        //    }
        //}
        public void outputMap()
        {
            string filePath = @"C:\Users\computing\Documents\CodenameDragon\output.csv";
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            string[][] output = new string[dimensions.X][];
            for (int i = 0; i < dimensions.X; ++i)
            {
                output[i] = new string[dimensions.Y];
                for (int j = 0; j < dimensions.Y; ++j)
                {
                    output[i][j] = mapArray[i, j].ToString();
                }
            }

            int length = output.GetLength(0);

            for (int index = 0; index < length; index++)
            {
                sb.AppendLine(string.Join(delimiter, output[index]));
                File.WriteAllText(filePath, sb.ToString());
            }
        }
        



    }
}
