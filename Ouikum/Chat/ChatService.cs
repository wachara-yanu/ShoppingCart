using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;
using Ouikum;
using Prosoft.Service;
using System.Transactions;
namespace Ouikum.Common
{
    public class ChatService : BaseSC
    {

        public string _RoomCode { get; set; }
        public int _RoomID { get; set; }
        public int SumUnRead { get; set; }

        public enum Period
        {
            Topten,
            All,
            ToDay,
            Yesterday,
            last_7_days,
            This_week,
            Last_week,
            Last_30_Days,
            This_month,
            Last_month,
            This_year,
            Last_year
        }

        #region Chat

        #region Message

        #region SqlWherePeriod
        public string SqlWherePeriod(Period _period = Period.ToDay)
        {
            var sqlWhere = string.Empty;
            if (_period == Period.ToDay)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( d, CreatedDate, GETDATE() ) = 0)";
            }
            else if (_period == Period.Yesterday)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( d, CreatedDate, GETDATE() ) = 1)";
            }
            else if (_period == Period.last_7_days)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( d, CreatedDate, GETDATE() ) < 7)";
            }
            else if (_period == Period.This_week)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddDays(-7).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( ww, CreatedDate, GETDATE() ) = 0 )";
            }
            else if (_period == Period.Last_week)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddDays(-7).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( ww, CreatedDate, GETDATE() ) = 1 )";
            }
            else if (_period == Period.Last_30_Days)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddDays(-7).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( d, CreatedDate, GETDATE() ) < 30)";
            }
            else if (_period == Period.This_month)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddMonths(-1).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( m, CreatedDate, GETDATE() ) = 0)";
            }
            else if (_period == Period.Last_month)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddMonths(-1).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( m, CreatedDate, GETDATE() ) = 1)";
            }
            else if (_period == Period.This_year)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddYears(-1).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( yy, CreatedDate, GETDATE() ) = 0)";
            }
            else if (_period == Period.Last_year)
            {
                //sqlWhere = " (CreatedDate Between '" + DateTime.Now.AddYears(-1).ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59')";
                sqlWhere = " (DATEDIFF( yy, CreatedDate, GETDATE() ) = 1)";
            }
            return sqlWhere;
        }
        #endregion

        #region ReadingMessage
        public bool ReadingMessage(int ID, int RoomID)
        {
            TotalRow = CountData<emChatRoom>(" * ", " IsDelete = 0 AND RoomID = N'" + RoomID + "' ");
            if (TotalRow > 0)
                UpdateByCondition<emChatMessage>(" IsRead = 1 ", " IsDelete = 0 AND ToID = " + ID + " AND RoomID = " + RoomID + " AND IsRead = 0 ");
            else
                IsResult = false;

            return IsResult;
        }
        #endregion

        #region CheckANDGetMessage
        public List<view_emChat> CheckANDGetMessage(int ID, string RoomCode)
        {
            if (CheckExistUserInRoom(ID, RoomCode))
            {
                return GetMessage(RoomCode);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region GetMessage
        public List<view_emChat> GetMessage(string RoomCode, Period _period = Period.ToDay)
        {
            List<view_emChat> data = new List<view_emChat>();
            TotalRow = CountData<view_emChat>(" * ", "IsDelete= 0 AND RoomCode = N'" + RoomCode + "'");

            if (TotalRow > 0)
            {
                var sqlWhere = "IsDelete= 0 AND RoomCode = N'" + RoomCode + "'";
                if (_period != Period.All && _period != Period.Topten)
                {
                    sqlWhere += "  AND ";
                    sqlWhere += SqlWherePeriod(_period);
                }
                if (_period != Period.Topten)
                {
                    data = SelectData<view_emChat>(" * ", sqlWhere);
                }
                else
                {
                    sqlWhere += " AND (IsRead = 0 OR ";
                    sqlWhere += SqlWherePeriod(Period.ToDay);
                    sqlWhere += ")";
                    data = SelectData<view_emChat>(" * ", sqlWhere, null, 1, 10);
                }
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region SaveMessage
        public bool SaveMessage(int RoomID, int FromID, int ToID, string Message)
        {
            var model = new emChatMessage();
            model.RoomID = RoomID;
            model.FromID = FromID;

            model.ToID = ToID;
            model.Message = Message;
            model.CreatedDate = DateTimeNow;
            model.ModifiedDate = DateTimeNow;
            model.CreatedBy = "" + FromID;
            model.ModifiedBy = "" + FromID;
            model.IsDelete = false;
            model.IsRead = false;
            model.IsShow = true;
            model.RowVersion = 1;
            SaveData<emChatMessage>(model, "ChatID");
            return IsResult;
        }
        #endregion

        #region GetMessageNotRead
        public List<view_ChatMessageReadCount> GetMessageNotRead(int ID)
        {
            var sqlWhere = " IsRead = 0 AND  ToID = " + ID;
            var data = SelectData<view_ChatMessageReadCount>(" * ", sqlWhere, " ReadCount DESC ");
            return data;
        }
        #endregion


        #region GetMessageRead
        public List<view_ChatMessageReadCount> GetMessageRead(int ID)
        {
            var sqlWhere = "FromID = " + ID;
            var data = SelectData<view_ChatMessageReadCount>(" * ", sqlWhere, " IsRead asc ");
            return data;
        }
        #endregion

        #region SelectCompMessage
        public string SelectCompMessage(int id)
        {
            ChatService svChat = new ChatService();
            var comp = svChat.SelectData<b2bCompany>("CompID,CompName", "IsDelete = 0 AND CompID = " + id, "", 1, 1).First();
            return comp.CompName;
        }
        #endregion

        #endregion

        #region Room
        #region CreateRoom
        /// <summary>
        /// สร้าง ห้องคุย
        /// </summary>
        /// <param name="FromID"></param>
        /// <param name="ToID"></param>
        /// <returns>bool</returns>
        public bool CreateRoom(int FromID, int ToID)
        {
            var model = new emChatRoom();
            var encrypt = new EncryptManager();
            _RoomCode = encrypt.EncryptData(FromID + "-" + ToID);
            model.RoomCode = _RoomCode;
            model.RowFlag = 1;
            model.IsDelete = false;
            model.IsShow = true;
            model.CreatedBy = "" + FromID;
            model.ModifiedBy = "" + FromID;
            model.CreatedDate = DateTimeNow;
            model.ModifiedDate = DateTimeNow;
            model.RowVersion = 1;
            SaveData<emChatRoom>(model, "RoomID");
            _RoomID = model.RoomID;
            return IsResult;
        }
        #endregion

        #region CheckExistUserInRoom
        /// <summary>
        /// check ว่า ID ตรงกับ room นี้ไหม
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="RoomCode"></param>
        /// <returns>bool</returns>
        public bool CheckExistUserInRoom(int ID, string RoomCode)
        {
            var data = CountData<emChatRoom>(" * ", " IsDelete = 0 AND RoomCode = N'" + RoomCode + "' ");

            if (data > 0)
                ChectExistIDInRoom(ID, RoomCode);
            else
                IsResult = false;

            return IsResult;
        }
        #endregion

        #region CheckExistUserInRoomAndCreateIfNot
        /// <summary>
        /// check ว่า เคยมี room นี้ไหม ถ้าไม่มี ให้สร้าง room ใหม่
        /// </summary>
        /// <param name="fromID"></param>
        /// <param name="toID"></param>
        /// <returns>bool</returns>
        public bool CheckExistUserInRoomAndCreateIfNot(int fromID, int toID)
        {
            var listID = new List<int>();
            var encrypt = new EncryptManager();
            var Code_f = encrypt.EncryptData(fromID + "-" + toID);
            var Code_t = encrypt.EncryptData(toID + "-" + fromID);
            listID.Add(fromID);
            listID.Add(toID);
            var room = SelectData<emChatRoom>(" * ", " IsDelete = 0 AND (RoomCode = N'" + Code_f + "' OR RoomCode = N'" + Code_t + "')");

            if (TotalRow > 0)
            {
                var data = room.First();
                _RoomCode = data.RoomCode;
                _RoomID = data.RoomID;
                IsResult = ChectExistIDInRoom(listID, _RoomCode);
            }
            else
            {
                IsResult = CreateRoom(fromID, toID);
            }

            IsResult = CreateFriend(fromID, toID);

            return IsResult;
        }
        #endregion

        #region ChectExistIDInRoom
        /// <summary>
        /// check  id มีอยู่ใน roomcode
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="RoomCode"></param>
        /// <returns>bool</returns>
        private bool ChectExistIDInRoom(List<int> ListID, string RoomCode)
        {
            foreach (var item in ListID)
            {
                IsResult = CheckExistUserInRoom(item, RoomCode);
            }
            return IsResult;

        }

        private bool ChectExistIDInRoom(int ID, string RoomCode)
        {
            var encrypt = new EncryptManager();
            var code = encrypt.DecryptData(RoomCode);
            var listSplit = code.Split('-');
            foreach (var splt in listSplit)
            {
                if (int.Parse(splt) == ID)
                {
                    IsResult = true;
                    break;
                }
                else
                {
                    IsResult = false;
                }
            }
            return IsResult;
        }
        #endregion
        #endregion

        #region Friend
        #region GetFriend
        public List<view_ChatFriend> GetFriend(int LogonCompID)
        {
            List<view_ChatFriend> ChatFriends = new List<view_ChatFriend>();
            var FriendID = SelectData<view_ChatFriend>("Distinct CompID,FriendID ", " CompID = " + LogonCompID,null,1,0,false);
            if (FriendID.Count() > 0)
            {
                foreach (var it in (List<view_ChatFriend>)FriendID)
                {
                    var Friend = SelectData<view_ChatFriend>(" * ", " FriendID = " + it.FriendID).First();
                    ChatFriends.Add(Friend);
                }
                return ChatFriends;
            }
            else {
                return null;
            }
        }
        #endregion

        #region CreateFriend
        public bool CreateFriend(int FromID, int ToID)
        {

            using (var trans = new TransactionScope())
            {
                var model = new emChatFriend();

                #region Check & Save Friend
                if (!CheckExistFriend(FromID, ToID))
                {
                    model.CompID = FromID;
                    model.FriendID = ToID;
                    model.RowFlag = 1;
                    model.IsDelete = false;
                    model.IsShow = true;
                    model.CreatedBy = "" + FromID;
                    model.ModifiedBy = "" + FromID;
                    model.CreatedDate = DateTimeNow;
                    model.ModifiedDate = DateTimeNow;
                    model.RowVersion = 1;
                    SaveData<emChatFriend>(model, "FriendChatID");
                }
                #endregion

                #region Check & Save Friend
                if (!CheckExistFriend(ToID, FromID))
                {
                    model = new emChatFriend();
                    model.CompID = ToID;
                    model.FriendID = FromID;
                    model.RowFlag = 1;
                    model.IsDelete = false;
                    model.IsShow = true;
                    model.CreatedBy = "" + FromID;
                    model.ModifiedBy = "" + FromID;
                    model.CreatedDate = DateTimeNow;
                    model.ModifiedDate = DateTimeNow;
                    model.RowVersion = 1;
                    SaveData<emChatFriend>(model, "FriendChatID");
                }
                #endregion

                trans.Complete();
            }



            return IsResult;
        }
        #endregion

        #region CheckExistFriend Before Save
        public bool CheckExistFriend(int FromID, int ToID)
        {
            var sqlWhere = " ( CompID = " + FromID + " AND FriendID = " + ToID + "  ) ";
            var count = CountData<emChatFriend>(" * ", sqlWhere);

            if (count > 0)
                IsResult = true;
            else
                IsResult = false;

            return IsResult;
        }
        #endregion
        #endregion

        #endregion

    }
}

