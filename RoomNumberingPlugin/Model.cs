using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomNumberingPlugin
{
    public class Model
    {
        public static List<Room> GetAllRooms(Document document)
        {
            return new FilteredElementCollector(document)
                   .OfCategory(BuiltInCategory.OST_Rooms)
                   .OfType<Room>().ToList();
        }

        public static List<Level> GetUniqueLevelOfRooms(Document document, List<Room> roomList)
        {
            List<Level> levelList = new List<Level>();
            List<ElementId> LevelIdList = new List<ElementId>();
            foreach (Room room in roomList)
            {
                LevelIdList.Add(room.LevelId);
            }
            List<ElementId> UniqueLevelIdList = LevelIdList.Distinct().ToList();
            foreach (ElementId elemid in UniqueLevelIdList)
            {
                if (elemid.IntegerValue > 0)
                    levelList.Add(document.GetElement(elemid) as Level);
                continue;
            }

            return levelList;
        }


    }
}
