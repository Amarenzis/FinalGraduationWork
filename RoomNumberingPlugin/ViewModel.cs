using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomNumberingPlugin
{
    public class ViewModel
    {
        private ExternalCommandData vm_commandData;
        private Document vm_doc;

        public List<Level> RoomLevel { get; } = new List<Level>();

        public Level SelectedRoomLevel { get; set; }
        public bool YIsChecked { get; set; }
        public int StartValue { get; set; }

        public DelegateCommand RenameCommand { get; }

        public ViewModel(ExternalCommandData commandData)
        {
            vm_commandData = commandData;
            vm_doc = vm_commandData.Application.ActiveUIDocument.Document;

            //Создаем с помощью класса Model список всех комнат
            List<Room> roomList = Model.GetAllRooms(vm_doc);
            if (roomList == null)
                TaskDialog.Show("Ошибка", "В модели не существует помещений");

            //Забираем уникальные уровни через Id, иначе не работает
            RoomLevel = Model.GetUniqueLevelOfRooms(vm_doc, roomList);

            //Команда prism
            RenameCommand = new DelegateCommand(AddRenameCommand);

        }

        private void AddRenameCommand()
        {
            //Забираем комнаты выделенного этажа
            List<Room> roomListOnLevel = Model.GetAllRooms(vm_doc).Where(x => x.LevelId.Equals(SelectedRoomLevel.Id)).ToList();

            //Создаём словарь где ключ - центр комнаты, а значение - комната.
            Dictionary<XYZ, Room> kPointVRoomDictionary = new Dictionary<XYZ, Room>();
            foreach (Room room in roomListOnLevel)
            {
                BoundingBoxXYZ roomBoundingBox = room.get_BoundingBox(null);
                XYZ roomCenter = (roomBoundingBox.Max + roomBoundingBox.Min) / 2;
                kPointVRoomDictionary.Add(roomCenter, room);
            }

            //Получаем сортированный список комнат
            List<Room> roomOrderedList = new List<Room>();            
            if (YIsChecked)
                roomOrderedList = kPointVRoomDictionary.OrderBy(x => x.Key.Y)
                                                       .ThenBy(x => x.Key.X)
                                                       .Select(x => x.Value)
                                                       .ToList();
            else
                roomOrderedList = kPointVRoomDictionary.OrderBy(x => x.Key.X)
                                                       .ThenBy(x => x.Key.Y)
                                                       .Select(x => x.Value)
                                                       .ToList();
            int counter = 0;

            //Начинаем транзакцию
            using (Transaction ts = new Transaction(vm_doc, "Перенумерация комнат"))
            {
                ts.Start();

                foreach (Room room in roomOrderedList)
                {
                    Parameter numberParameter = room.get_Parameter(BuiltInParameter.ROOM_NUMBER);
                    numberParameter.Set((StartValue + counter).ToString());
                    counter++;
                }

                ts.Commit();
            }            

            RalseCloseRequest();
        }

        public event EventHandler CloseRequest;

        private void RalseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
