using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Jocasta.Providers
{
    public class HelperProvider
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
        public static bool CheckImagePropertyId(Image img)
        {
            for (int indexPropertyId = 0; indexPropertyId < img.PropertyIdList.Length; indexPropertyId++)
            {
                if (img.PropertyIdList[indexPropertyId] == 0x0112) return true;
            }
            return false;
        }
        public static bool SaveResizeImage(Image img, int width, string path)
        {
            try
            {

                if (CheckImagePropertyId(img))
                {

                    int rotationValue = img.GetPropertyItem(0x0112).Value[0];
                    switch (rotationValue)
                    {
                        case 1: // landscape, do nothing
                            break;

                        case 8: // rotated 90 right
                                // de-rotate:
                            img.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                            break;

                        case 3: // bottoms up
                            img.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                            break;

                        case 6: // rotated 90 left
                            img.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                            break;
                    }
                }

                int originalW = img.Width;
                int originalH = img.Height;
                int resizedW = originalW;
                int resizedH = originalH;
                if (originalW > width)
                {
                    resizedW = width;
                    resizedH = (originalH * resizedW) / originalW;
                }
                Bitmap b = new Bitmap(resizedW, resizedH);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;    // Specify here
                g.DrawImage(img, 0, 0, resizedW, resizedH);
                g.Dispose();
                b.Save(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string EncodePassword(string userId, string password)
        {
            try
            {
                userId = CreateMD5(userId);
                password = CreateMD5(userId[userId.Length - 1] + password + userId[0]);
                password = CreateMD5(userId[userId.Length - 2] + password + userId[1]);
                return password;
            }
            catch (Exception ex) { return null; }
        }

        public static string CreateToken(string userId, string password)
        {
            try
            {
                string token = "";
                token = Guid.NewGuid().ToString();
                token = Base64Encode(token);

                //Mã Id
                token = token.Substring(0, 10) + "vkK9@3FH3*Y67!" + userId + "vkK9@3FH3*Y67!" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Mã hoá password
                token = token.Substring(0, 10) + "9aB67$$W07XoKd" + password + "9aB67$$W07XoKd" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);

                //Mã hóa thời gian
                token = token.Substring(0, 10) + "uEiq*8f@93GXj" + MakeCode() + "uEiq*8f@93GXj" + token.Substring(10, token.Length - 10);
                token = Base64Encode(token);


                //Ma hoa lan cuoi
                for (int i = 0; i < 3; i++)
                {
                    token = token + Guid.NewGuid().ToString();
                    token = Base64Encode(token);
                }

                token = "SmartLife " + token;
                return token;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
        public static string MakeCode()
        {
            try
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                ulong time = (ulong)(DateTime.UtcNow - epoch).TotalMilliseconds;

                char[] _base = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V' };
                char[] Code = new char[10];

                Code[0] = _base[(time >> 45) & 31];
                Code[1] = _base[(time >> 40) & 31];
                Code[2] = _base[(time >> 35) & 31];
                Code[3] = _base[(time >> 30) & 31];
                Code[4] = _base[(time >> 25) & 31];
                Code[5] = _base[(time >> 20) & 31];
                Code[6] = _base[(time >> 15) & 31];
                Code[7] = _base[(time >> 10) & 31];
                Code[8] = _base[(time >> 5) & 31];
                Code[9] = _base[time & 31];

                string ShareCode = new string(Code);

                return ShareCode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static long GetSeconds(DateTime? dateTime = null)
        {
            try
            {
                if (!dateTime.HasValue) dateTime = DateTime.UtcNow;
                var Timestamp = new DateTimeOffset(dateTime.Value).ToUnixTimeMilliseconds();
                return Timestamp;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string PrettyNumber(decimal number)
        {
            string strNumber = number.ToString("G29");
            if (strNumber.IndexOf('.') > -1) { strNumber = long.Parse(strNumber.Split('.')[0]).ToString("N0") + "." + strNumber.Split('.')[1]; }
            else { strNumber = number.ToString("N0"); }
            return strNumber;
        }
        public static void Base64ToImage(string base64String, string pathToSave)
        {
            try
            {
                List<string> listPartFolder = pathToSave.Split('\\').ToList();
                listPartFolder.RemoveAt(listPartFolder.Count - 1);
                string director = string.Join("\\", listPartFolder);
                if (!Directory.Exists(director)) Directory.CreateDirectory(director);
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    Bitmap bmp = new Bitmap(image);
                    bmp.Save(pathToSave);
                }
            }
            catch (Exception ex) { throw ex; }



        }

        public static DateTime GetDateTime(long time)
        {
            double sticks = double.Parse(time.ToString());
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(sticks);
            DateTime dateTime = new DateTime(1970, 1, 1,7,0,0,0) + timeSpan;
            return dateTime;
        }

        public static DateTime GetDateTime_v2(long time)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime;
        }
        public static bool DeleteFile(string path)
        {
            try
            {
                //xoa anh cu
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(path));
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public static string CreateSlugFromTitle(string text)
        {
            string[] arr1 = new string[] { "~","`","#","^","&","*","(",")","+","=","<",">","|","[","]","%",",","@","_","?",".","$","/"," ","á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","-","a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
    }
}