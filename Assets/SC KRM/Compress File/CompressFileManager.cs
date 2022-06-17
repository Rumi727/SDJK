using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using SCKRM.Threads;
using UnityEngine;

namespace SCKRM.Compress
{
    public static class CompressFileManager
    {
        /// <summary>
        /// 파일을 압축합니다
        /// </summary>
        /// <param name="sourceDirectory">
        /// 압축 할 파일의 경로 입니다
        /// </param>
        /// <param name="zipFilePath">
        /// 압축 된 파일을 저장 할 경로 입니다
        /// </param>
        /// <param name="password">
        /// 압축 파일의 암호를 결정합니다
        /// </param>
        /// <param name="threadMetaData">
        /// 스레드에서 실행했을때를 대비한 인자입니다 (즉, 이 메소드는 스레드에 안전합니다 아마도요)
        /// </param>
        /// <returns>
        /// 압축이 성공했는가의 여부입니다
        /// </returns>
        public static bool CompressZipFile(string sourceDirectory, string zipFilePath, string password = "", ThreadMetaData threadMetaData = null)
        {
            int stopLoop = 0;

            //폴더가 존재하는 경우에만 수행
            if (Directory.Exists(sourceDirectory))
            {
                //압축 대상 폴더의 파일 목록
                List<string> fileList = GenerateFileList(sourceDirectory);

                //압축 대상 폴더 경로의 길이 + 1
                int TrimLength = (Directory.GetParent(sourceDirectory)).ToString().Length + 1;

                //find number of chars to remove. from orginal file path. remove '\'
                FileStream ostream;
                byte[] obuffer;
                string outPath = zipFilePath;

                //ZIP 스트림 생성
                using ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath));

                try
                {
                    //패스워드가 있는 경우 패스워드 지정
                    if (password != null && password != string.Empty)
                        oZipStream.Password = password;

                    oZipStream.SetLevel(9); //암호화 레벨 (최대 압축)

                    if (threadMetaData != null)
                    {
                        threadMetaData.name = "compress_file_manager.compress";
                        threadMetaData.info = "";

                        threadMetaData.progress = 0;
                        threadMetaData.maxProgress = fileList.Count;

                        threadMetaData.cancelEvent += CancelEvent;
                        threadMetaData.cantCancel = false;
                    }

                    ZipEntry oZipEntry;
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        Interlocked.Decrement(ref stopLoop);
                        if (Interlocked.Increment(ref stopLoop) > 0)
                        {
                            oZipStream.Close();
                            return false;
                        }

                        string Fil = fileList[i];
                        oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                        oZipStream.PutNextEntry(oZipEntry);

                        //파일인 경우
                        if (!Fil.EndsWith(@"/"))
                        {
                            ostream = File.OpenRead(Fil);
                            obuffer = new byte[ostream.Length];
                            ostream.Read(obuffer, 0, obuffer.Length);
                            oZipStream.Write(obuffer, 0, obuffer.Length);
                        }

                        if (threadMetaData != null)
                        {
                            threadMetaData.info = fileList[i];
                            threadMetaData.progress = i + 1;
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    //오류가 난 경우 생성 했던 파일을 삭제
                    if (File.Exists(outPath))
                        File.Delete(outPath);

                    Debug.LogException(e);

                    return false;
                }
                finally
                {
                    threadMetaData.info = "";

                    //압축 종료
                    oZipStream.Finish();
                    oZipStream.Close();
                }
            }

            return false;



            void CancelEvent()
            {
                Interlocked.Increment(ref stopLoop);

                threadMetaData.maxProgress = 1;
                threadMetaData.progress = 1;

                threadMetaData.cancelEvent -= CancelEvent;
            }
        }

        static List<string> GenerateFileList(string Dir)
        {
            List<string> fils = new List<string>();
            bool Empty = true;

            //폴더 내의 파일 추가
            string[] filePaths = Directory.GetFiles(Dir);
            for (int i = 0; i < filePaths.Length; i++)
            {
                string file = filePaths[i];
                fils.Add(file);
                Empty = false;
            }

            //파일이 없고, 폴더도 없는 경우 자신의 폴더 추가
            if (Empty && Directory.GetDirectories(Dir).Length == 0)
                fils.Add(Dir + @"/");

            //폴더 내 폴더 목록.
            string[] paths = Directory.GetDirectories(Dir);
            for (int i = 0; i < paths.Length; i++)
            {
                string dirs = paths[i];
                //해당 폴더로 다시 GenerateFileList 재귀 호출
                List<string> generateFileList = GenerateFileList(dirs);
                for (int i1 = 0; i1 < generateFileList.Count; i1++)
                    //해당 폴더 내의 파일, 폴더 추가.
                    fils.Add(generateFileList[i1]);
            }

            return fils;
        }

        /// <summary>
        /// 압축을 해제합니다
        /// </summary>
        /// <param name="zipFilePath">
        /// 압축을 해제할 파일의 경로입니다
        /// </param>
        /// <param name="targetDirectory">
        /// 압축을 해제하고 결과를 저장할 경로입니다
        /// </param>
        /// <param name="password">
        /// 압축 파일의 암호를 결정합니다
        /// </param>
        /// <param name="threadMetaData">
        /// 스레드에서 실행했을때를 대비한 인자입니다 (즉, 이 메소드는 스레드에 안전합니다 아마도요)
        /// </param>
        /// <returns></returns>
        public static bool DecompressZipFile(string zipFilePath, string targetDirectory, string password = "", ThreadMetaData threadMetaData = null)
        {
            int stopLoop = 0;

            //ZIP 파일이 있는 경우만 수행
            if (File.Exists(zipFilePath))
            {
                using (ZipFile zipFile = new ZipFile(File.OpenRead(zipFilePath)))
                {
                    if (threadMetaData != null)
                    {
                        threadMetaData.name = "compress_file_manager.decompress";
                        threadMetaData.info = "";

                        threadMetaData.progress = 0;
                        threadMetaData.maxProgress = zipFile.Count;

                        threadMetaData.cancelEvent += CancelEvent;
                        threadMetaData.cantCancel = false;
                    }
                }

                //ZIP 스트림 생성.
                using ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(zipFilePath));

                //패스워드가 있는 경우 패스워드 지정
                if (password != null && password != string.Empty)
                    zipInputStream.Password = password;

                try
                {
                    ZipEntry theEntry;
                    //반복하며 파일을 가져옴.
                    while ((theEntry = zipInputStream.GetNextEntry()) != null)
                    {
                        Interlocked.Decrement(ref stopLoop);
                        if (Interlocked.Increment(ref stopLoop) > 0)
                        {
                            zipInputStream.Close();
                            return false;
                        }

                        //폴더
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name); // 파일

                        //폴더 생성
                        Directory.CreateDirectory(PathTool.Combine(targetDirectory, directoryName));

                        //파일 이름이 있는 경우
                        if (fileName != string.Empty)
                        {
                            //파일 스트림 생성 (파일생성)
                            using FileStream streamWriter = File.Create(PathTool.Combine(targetDirectory, theEntry.Name));

                            int size = 2048;
                            byte[] data = new byte[2048];

                            //파일 복사
                            while (true)
                            {
                                Interlocked.Decrement(ref stopLoop);
                                if (Interlocked.Increment(ref stopLoop) > 0)
                                {
                                    streamWriter.Close();
                                    zipInputStream.Close();
                                    return false;
                                }

                                size = zipInputStream.Read(data, 0, data.Length);

                                if (size > 0)
                                    streamWriter.Write(data, 0, size);
                                else
                                    break;
                            }

                            //파일스트림 종료
                            streamWriter.Close();

                            if (threadMetaData != null)
                            {
                                threadMetaData.info = theEntry.Name;
                                threadMetaData.progress++;
                            }
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
                finally
                {
                    threadMetaData.info = "";

                    //ZIP 파일 스트림 종료
                    zipInputStream.Close();
                }
            }

            return false;



            void CancelEvent()
            {
                Interlocked.Increment(ref stopLoop);

                threadMetaData.maxProgress = 1;
                threadMetaData.progress = 1;
            }
        }
    }
}