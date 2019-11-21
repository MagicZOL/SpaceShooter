using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//파일의 입출력을 위한 네임스페이스
using System.IO;
//바이너리 파일 포맷을 위한 네임스페이스
using System.Runtime.Serialization.Formatters.Binary;
//데이터 저장 클래스에 접근하기 위한 네임스페이스
using DataInfo;

public class DataManager : MonoBehaviour
{
    //파일이 저장될 물리적인 경로 및 파일명을 저장할 변수
    private string dataPath;

    //파일 저장 경로와 파일명 지정
    public void Initialize()
    {
        dataPath = Application.persistentDataPath + "/gameData.dat";
        //persistentDataPath : 저장할수있는 적합한 Path를 반환
        //개발자가 원하는 폴더에 저장할 수 있는게 아니라 운영체제가 지정한 곳에 저장해야하므로
        //저장할수 있는 공간을 알려주는 함수가 필요하다. 운영체제마다 함수가 다르다.
    }

    //데이터 저장 및 파일을 생성하는 함수
    public void Save(GameData gameData)
    {
        //바이너리 파일 포맷을 위한 BinaryFormatter 생성
        BinaryFormatter bf = new BinaryFormatter();
        //데이터 저장을 위한 파일 생성
        FileStream file = File.Create(dataPath);

        //파일에 저장할 클래스 데이터 할당
        GameData data = new GameData();
        data.killCount = gameData.killCount;
        data.hp = gameData.hp;
        data.speed = gameData.speed;
        data.damage = gameData.damage;
        data.equipItem = gameData.equipItem;

        //BinaryFormatter를 사용해 파일에 데이터 기록
        bf.Serialize(file, data);
        file.Close();
    }

    //파일에서 데이터를 추출하는 함수
    public GameData Load()
    {
        if (File.Exists(dataPath))
        {
            //파일이 존재할 경우 데이터 불러오기
            BinaryFormatter bf = new BinaryFormatter();
            GameData data = null;

            //FileStream file = File.Open(dataPath, FileMode.Open);
            ////GameData 클래스에 파일로부터 읽은 데이터를 기록
            //data = (GameData)bf.Deserialize(file);
            //file.Close();

            //try,catch문 try문에서 오류가 나면 catch문으로 넘어가서 실행한다.
            try
            {
                FileStream file = File.Open(dataPath, FileMode.Open); //파일이 꺠져서 오류
                data = (GameData)bf.Deserialize(file);
                file.Close();
            }
            catch (IOException e)
            {
                File.Delete(dataPath); //파일삭제,데이터 날라감...
                data = new GameData();
            }

            return data;
        }
        else
        {
            //파일이 없을 경우 기본값을 반환
            GameData data = new GameData();

            return data;
        }
    }
}
