using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bummerman
{
    class Dungeon
    {
        class Room
        {
	        // these values hold grid coordinates for each corner of the room
	        public int x1;
	        public int x2;
	        public int y1;
	        public int y2;

	        // width and height of room in terms of grid
	        public int w;
	        public int h;

	        // center point of the room
	        public Point center;

	        // constructor for creating new rooms
	        public Room (int x, int y, int w, int h) 
            {
                // Measurements are in tiles
		        x1 = x;
		        x2 = x + w;
		        y1 = y;
		        y2 = y + h;
		        this.w = w;
		        this.h = h;
		        center = new Point(
                    (int)Math.Floor((x1 + x2) / 2f),
                    (int)Math.Floor((y1 + y2) / 2f));
	        }

	        // return true if this room intersects another room
	        public bool Intersects(Room otherRoom) {
                return (x1 <= otherRoom.x2 && 
                    x2 >= otherRoom.x1 &&
                    y1 <= otherRoom.y2 && 
                    otherRoom.y2 >= otherRoom.y1
                );
	        }
            // End Room class
        }

        /// Storage for all the rooms
        List<Room> rooms;

        /// Extents of dungeon map
        const int mapWidth = 100;
        const int mapHeight = 100;

        public Dungeon(int totalRooms)
        {
            // Generate rooms
            PlaceRooms(totalRooms, 7, 15);
        }

        /// <summary>
        /// Place a number of rooms in random locations
        /// </summary>
        /// <returns></returns>
        void PlaceRooms(int totalRooms, int minRoomSize, int maxRoomSize) 
        {
		    // create array for room storage for easy access
		    rooms = new List<Room>(totalRooms);
            Random r = new Random();

		    // randomize values for each room
		    for (int i = 0; i < totalRooms; ++i) 
            {
                var w = minRoomSize + r.Next(maxRoomSize - minRoomSize + 1);
                var h = minRoomSize + r.Next(maxRoomSize - minRoomSize + 1);
			    var x = r.Next(mapWidth - w - 1) + 1;
                var y = r.Next(mapHeight - h - 1) + 1;

			    // create room with randomized values
			    var newRoom = new Room(x, y, w, h);

			    var failed = false;

			    foreach (Room otherRoom in rooms) {
				    if (newRoom.Intersects(otherRoom)) {
					    failed = true;
					    break;
				    }
			    }

                if (!failed)
                {
                    // Carve out new room and add it to the list
                    //CreateRoom(newRoom);
                    rooms.Add(newRoom);
                }
		    }
            // Finish placing rooms
	    }
    }
}
