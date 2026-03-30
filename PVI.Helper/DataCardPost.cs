
namespace PVI.Helper
{
    public class DataCardPost
    {
        public DataCardPost()
        {
        }
        public DataCardPost(string _Id, string _Type, string _RequestId, string _Sign, ImageData _DataContent)
        {
            Id = _Id;
            Type = _Type;
            RequestId = _RequestId;
            Sign = _Sign;
            DataContent = _DataContent;
        }

        private string _Id;
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        private string _Type;
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private string _RequestId;
        public string RequestId
        {
            get
            {
                return _RequestId;
            }
            set
            {
                _RequestId = value;
            }
        }

        private string _Sign;
        public string Sign
        {
            get
            {
                return _Sign;
            }
            set
            {
                _Sign = value;
            }
        }


        private ImageData _DataContent;
        public ImageData DataContent
        {
            get
            {
                return _DataContent;
            }
            set
            {
                _DataContent = value;
            }
        }
    }

    public class ImageData
    {
        public ImageData()
        {
        }
        public ImageData(string _img1, string _img2, string _img)
        {
            img1 = _img1;
            img2 = _img2;
            img = _img;
        }

        private string _img1;
        public string img1
        {
            get
            {
                return _img1;
            }
            set
            {
                _img1 = value;
            }
        }

        private string _img2;
        public string img2
        {
            get
            {
                return _img2;
            }
            set
            {
                _img2 = value;
            }
        }

        private string _img;
        public string img
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
            }
        }
    }

    public class VehicleInspectionContent
    {
        public VehicleInspectionContent()
        {
        }
        public VehicleInspectionContent(string _file_size, string _CpId, string _Sign)
        {
            file_size = _file_size;
            CpId = _CpId;
            Sign = _Sign;
        }

        private string _file_size;
        public string file_size
        {
            get
            {
                return _file_size;
            }
            set
            {
                _file_size = value;
            }
        }

        private string _CpId;
        public string CpId
        {
            get
            {
                return _CpId;
            }
            set
            {
                _CpId = value;
            }
        }

        private string _Sign;
        public string Sign
        {
            get
            {
                return _Sign;
            }
            set
            {
                _Sign = value;
            }
        }
    }

    public class ResultDrivingLicense
    {
        public ResultDrivingLicense()
        {
        }
        public ResultDrivingLicense(string _errorCode, string _errorMessage, DataContentDrivingLicense _data)
        {
            errorCode = _errorCode;
            errorMessage = _errorMessage;
            data = _data;
        }

        private string _errorCode;
        public string errorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }

        private string _errorMessage;
        public string errorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        private DataContentDrivingLicense _data;
        public DataContentDrivingLicense data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    public class ResultVehicleInspection
    {
        public ResultVehicleInspection()
        {
        }
        public ResultVehicleInspection(string _errorCode, string _errorMessage, DataContentVehicleInspection _data)
        {
            errorCode = _errorCode;
            errorMessage = _errorMessage;
            data = _data;
        }

        private string _errorCode;
        public string errorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }

        private string _errorMessage;
        public string errorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        private DataContentVehicleInspection _data;
        public DataContentVehicleInspection data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    public class DataContentVehicleInspection
    {
        public DataContentVehicleInspection()
        {
        }
        public DataContentVehicleInspection(string _type, ContentVehicleInspection _info)
        {
            type = _type;
            info = _info;
        }

        private string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private ContentVehicleInspection _info;
        public ContentVehicleInspection info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }
    }

    public class ContentVehicleInspection
    {
        public ContentVehicleInspection()
        {
        }
        public ContentVehicleInspection(string _chassis_number, string _engine_number, string _issued_on, string _manufactured_country, string _manufactured_year, string _mark, string _model_code, string _permissible_no, string _regis_date, string _registration_number, string _seri, string _type, string _valid_until, string _chassis_number_confidence, string _engine_number_confidence, string _issued_on_confidence, string _manufactured_country_confidence, string _manufactured_year_confidence, string _mark_confidence, string _model_code_confidence, string _permissible_no_confidence, string _regis_date_confidence, string _registration_number_confidence, string _seri_confidence, string _type_confidence, string _valid_until_confidence)
        {
            chassis_number = _chassis_number;
            engine_number = _engine_number;
            issued_on = _issued_on;
            manufactured_country = _manufactured_country;
            manufactured_year = _manufactured_year;
            mark = _mark;
            model_code = _model_code;
            permissible_no = _permissible_no;
            regis_date = _regis_date;
            registration_number = _registration_number;
            seri = _seri;
            type = _type;
            valid_until = _valid_until;
            chassis_number_confidence = _chassis_number_confidence;
            engine_number_confidence = _engine_number_confidence;
            issued_on_confidence = _issued_on_confidence;
            manufactured_country_confidence = _manufactured_country_confidence;
            manufactured_year_confidence = _manufactured_year_confidence;
            mark_confidence = _mark_confidence;
            model_code_confidence = _model_code_confidence;
            permissible_no_confidence = _permissible_no_confidence;
            regis_date_confidence = _regis_date_confidence;
            registration_number_confidence = _registration_number_confidence;
            seri_confidence = _seri_confidence;
            type_confidence = _type_confidence;
            valid_until_confidence = _valid_until_confidence;
        }

        private string _chassis_number;
        public string chassis_number
        {
            get
            {
                return _chassis_number;
            }
            set
            {
                _chassis_number = value;
            }
        }

        private string _engine_number;
        public string engine_number
        {
            get
            {
                return _engine_number;
            }
            set
            {
                _engine_number = value;
            }
        }

        private string _issued_on;
        public string issued_on
        {
            get
            {
                return _issued_on;
            }
            set
            {
                _issued_on = value;
            }
        }

        private string _manufactured_country;
        public string manufactured_country
        {
            get
            {
                return _manufactured_country;
            }
            set
            {
                _manufactured_country = value;
            }
        }

        private string _manufactured_year;
        public string manufactured_year
        {
            get
            {
                return _manufactured_year;
            }
            set
            {
                _manufactured_year = value;
            }
        }

        private string _mark;
        public string mark
        {
            get
            {
                return _mark;
            }
            set
            {
                _mark = value;
            }
        }

        private string _model_code;
        public string model_code
        {
            get
            {
                return _model_code;
            }
            set
            {
                _model_code = value;
            }
        }

        private string _permissible_no;
        public string permissible_no
        {
            get
            {
                return _permissible_no;
            }
            set
            {
                _permissible_no = value;
            }
        }

        private string _regis_date;
        public string regis_date
        {
            get
            {
                return _regis_date;
            }
            set
            {
                _regis_date = value;
            }
        }

        private string _registration_number;
        public string registration_number
        {
            get
            {
                return _registration_number;
            }
            set
            {
                _registration_number = value;
            }
        }

        private string _seri;
        public string seri
        {
            get
            {
                return _seri;
            }
            set
            {
                _seri = value;
            }
        }

        private string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private string _valid_until;
        public string valid_until
        {
            get
            {
                return _valid_until;
            }
            set
            {
                _valid_until = value;
            }
        }

        private string _chassis_number_confidence;
        public string chassis_number_confidence
        {
            get
            {
                return _chassis_number_confidence;
            }
            set
            {
                _chassis_number_confidence = value;
            }
        }

        private string _engine_number_confidence;
        public string engine_number_confidence
        {
            get
            {
                return _engine_number_confidence;
            }
            set
            {
                _engine_number_confidence = value;
            }
        }

        private string _issued_on_confidence;
        public string issued_on_confidence
        {
            get
            {
                return _issued_on_confidence;
            }
            set
            {
                _issued_on_confidence = value;
            }
        }

        private string _manufactured_country_confidence;
        public string manufactured_country_confidence
        {
            get
            {
                return _manufactured_country_confidence;
            }
            set
            {
                _manufactured_country_confidence = value;
            }
        }

        private string _manufactured_year_confidence;
        public string manufactured_year_confidence
        {
            get
            {
                return _manufactured_year_confidence;
            }
            set
            {
                _manufactured_year_confidence = value;
            }
        }

        private string _mark_confidence;
        public string mark_confidence
        {
            get
            {
                return _mark_confidence;
            }
            set
            {
                _mark_confidence = value;
            }
        }

        private string _model_code_confidence;
        public string model_code_confidence
        {
            get
            {
                return _model_code_confidence;
            }
            set
            {
                _model_code_confidence = value;
            }
        }

        private string _permissible_no_confidence;
        public string permissible_no_confidence
        {
            get
            {
                return _permissible_no_confidence;
            }
            set
            {
                _permissible_no_confidence = value;
            }
        }

        private string _regis_date_confidence;
        public string regis_date_confidence
        {
            get
            {
                return _regis_date_confidence;
            }
            set
            {
                _regis_date_confidence = value;
            }
        }

        private string _registration_number_confidence;
        public string registration_number_confidence
        {
            get
            {
                return _registration_number_confidence;
            }
            set
            {
                _registration_number_confidence = value;
            }
        }

        private string _seri_confidence;
        public string seri_confidence
        {
            get
            {
                return _seri_confidence;
            }
            set
            {
                _seri_confidence = value;
            }
        }

        private string _type_confidence;
        public string type_confidence
        {
            get
            {
                return _type_confidence;
            }
            set
            {
                _type_confidence = value;
            }
        }

        private string _valid_until_confidence;
        public string valid_until_confidence
        {
            get
            {
                return _valid_until_confidence;
            }
            set
            {
                _valid_until_confidence = value;
            }
        }
    }

    public class ResultVehicleRegistration
    {
        public ResultVehicleRegistration()
        {
        }
        public ResultVehicleRegistration(string _errorCode, string _errorMessage, List<DataConentVehicleRegistration> _data)
        {
            errorCode = _errorCode;
            errorMessage = _errorMessage;
            data = _data;
        }

        private string _errorCode;
        public string errorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }

        private string _errorMessage;
        public string errorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        private List<DataConentVehicleRegistration> _data;
        public List<DataConentVehicleRegistration> data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    public class VehicleRegistrationResult
    {
        public VehicleRegistrationResult()
        {
        }
        public VehicleRegistrationResult(string _Status, string _Message, List<DataConentVehicleRegistration> _data)
        {
            Status = _Status;
            Message = _Message;
            data = _data;
        }

        private string _Status;
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        private List<DataConentVehicleRegistration> _data;
        public List<DataConentVehicleRegistration> data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    public class DataConentVehicleRegistration
    {
        public DataConentVehicleRegistration()
        {
        }
        public DataConentVehicleRegistration(string _type, ContentVehicleRegistration _info)
        {
            type = _type;
            info = _info;
        }

        private string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private ContentVehicleRegistration _info;
        public ContentVehicleRegistration info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }
    }

    public class ContentVehicleRegistration
    {
        public ContentVehicleRegistration()
        {
        }
        public ContentVehicleRegistration(string _plate, string _name, string _address, string _first_issue_date, string _brand, string _color, string _issued_at, string _last_issue_date, string _model, string _plate_confidence, string _name_confidence, string _address_confidence, string _first_issue_date_confidence, string _brand_confidence, string _color_confidence, string _issued_at_confidence, string _last_issue_date_confidence, string _model_confidence, string _chassis, string _chassis_confidence, string _engine, string _engine_confidence, string _capacity, string _capacity_confidence)
        {
            plate = _plate;
            name = _name;
            address = _address;
            first_issue_date = _first_issue_date;
            brand = _brand;
            color = _color;
            issued_at = _issued_at;
            last_issue_date = _last_issue_date;
            model = _model;
            plate_confidence = _plate_confidence;
            name_confidence = _name_confidence;
            address_confidence = _address_confidence;
            first_issue_date_confidence = _first_issue_date_confidence;
            brand_confidence = _brand_confidence;
            color_confidence = _color_confidence;
            issued_at_confidence = _issued_at_confidence;
            last_issue_date_confidence = _last_issue_date_confidence;
            model_confidence = _model_confidence;
            chassis = _chassis;
            chassis_confidence = _chassis_confidence;
            engine = _engine;
            engine_confidence = _engine_confidence;
            capacity = _capacity;
            capacity_confidence = _capacity_confidence;
        }

        private string _plate;
        public string plate
        {
            get
            {
                return _plate;
            }
            set
            {
                _plate = value;
            }
        }

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _address;
        public string address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        private string _first_issue_date;
        public string first_issue_date
        {
            get
            {
                return _first_issue_date;
            }
            set
            {
                _first_issue_date = value;
            }
        }

        private string _brand;
        public string brand
        {
            get
            {
                return _brand;
            }
            set
            {
                _brand = value;
            }
        }

        private string _color;
        public string color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        private string _issued_at;
        public string issued_at
        {
            get
            {
                return _issued_at;
            }
            set
            {
                _issued_at = value;
            }
        }

        private string _last_issue_date;
        public string last_issue_date
        {
            get
            {
                return _last_issue_date;
            }
            set
            {
                _last_issue_date = value;
            }
        }

        private string _model;
        public string model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }

        private string _plate_confidence;
        public string plate_confidence
        {
            get
            {
                return _plate_confidence;
            }
            set
            {
                _plate_confidence = value;
            }
        }

        private string _name_confidence;
        public string name_confidence
        {
            get
            {
                return _name_confidence;
            }
            set
            {
                _name_confidence = value;
            }
        }

        private string _address_confidence;
        public string address_confidence
        {
            get
            {
                return _address_confidence;
            }
            set
            {
                _address_confidence = value;
            }
        }

        private string _first_issue_date_confidence;
        public string first_issue_date_confidence
        {
            get
            {
                return _first_issue_date_confidence;
            }
            set
            {
                _first_issue_date_confidence = value;
            }
        }

        private string _brand_confidence;
        public string brand_confidence
        {
            get
            {
                return _brand_confidence;
            }
            set
            {
                _brand_confidence = value;
            }
        }

        private string _color_confidence;
        public string color_confidence
        {
            get
            {
                return _color_confidence;
            }
            set
            {
                _color_confidence = value;
            }
        }

        private string _issued_at_confidence;
        public string issued_at_confidence
        {
            get
            {
                return _issued_at_confidence;
            }
            set
            {
                _issued_at_confidence = value;
            }
        }

        private string _last_issue_date_confidence;
        public string last_issue_date_confidence
        {
            get
            {
                return _last_issue_date_confidence;
            }
            set
            {
                _last_issue_date_confidence = value;
            }
        }

        private string _model_confidence;
        public string model_confidence
        {
            get
            {
                return _model_confidence;
            }
            set
            {
                _model_confidence = value;
            }
        }

        private string _chassis;
        public string chassis
        {
            get
            {
                return _chassis;
            }
            set
            {
                _chassis = value;
            }
        }

        private string _chassis_confidence;
        public string chassis_confidence
        {
            get
            {
                return _chassis_confidence;
            }
            set
            {
                _chassis_confidence = value;
            }
        }

        private string _engine;
        public string engine
        {
            get
            {
                return _engine;
            }
            set
            {
                _engine = value;
            }
        }

        private string _engine_confidence;
        public string engine_confidence
        {
            get
            {
                return _engine_confidence;
            }
            set
            {
                _engine_confidence = value;
            }
        }

        private string _capacity;
        public string capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                _capacity = value;
            }
        }

        private string _capacity_confidence;
        public string capacity_confidence
        {
            get
            {
                return _capacity_confidence;
            }
            set
            {
                _capacity_confidence = value;
            }
        }
    }

    public class DrivingLicenseContent
    {
        public DrivingLicenseContent()
        {
        }
        public DrivingLicenseContent(string _file_size, string _CpId, string _Sign)
        {
            file_size = _file_size;
            CpId = _CpId;
            Sign = _Sign;
        }

        private string _file_size;
        public string file_size
        {
            get
            {
                return _file_size;
            }
            set
            {
                _file_size = value;
            }
        }

        private string _CpId;
        public string CpId
        {
            get
            {
                return _CpId;
            }
            set
            {
                _CpId = value;
            }
        }

        private string _Sign;
        public string Sign
        {
            get
            {
                return _Sign;
            }
            set
            {
                _Sign = value;
            }
        }
    }

    public class DrivingLicenseResult
    {
        public DrivingLicenseResult()
        {
        }
        public DrivingLicenseResult(string _Status, string _Message, DataContentDrivingLicense _data)
        {
            Status = _Status;
            Message = _Message;
            data = _data;
        }

        private string _Status;
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        private DataContentDrivingLicense _data;
        public DataContentDrivingLicense data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    public class DataContentDrivingLicense
    {
        public DataContentDrivingLicense()
        {
        }
        public DataContentDrivingLicense(string _type, ContentDrivingLicense _info)
        {
            type = _type;
            info = _info;
        }

        private string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private ContentDrivingLicense _info;
        public ContentDrivingLicense info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }
    }

    public class ContentDrivingLicense
    {
        public ContentDrivingLicense()
        {
        }
        public ContentDrivingLicense(string _id, string _name, string _address, string _nationality, string _due_date, string _dob, string _issue_date, string _id_confidence, string _name_confidence, string _address_confidence, string _nationality_confidence, string _due_date_confidence, string _dob_confidence, string _issue_date_confidence, string _class_hang, string _class_hang_confidence)
        {
            id = _id;
            name = _name;
            address = _address;
            nationality = _nationality;
            due_date = _due_date;
            dob = _dob;
            issue_date = _issue_date;
            id_confidence = _id_confidence;
            name_confidence = _name_confidence;
            address_confidence = _address_confidence;
            nationality_confidence = _nationality_confidence;
            due_date_confidence = _due_date_confidence;
            dob_confidence = _dob_confidence;
            issue_date_confidence = _issue_date_confidence;
            class_hang = _class_hang;
            class_hang_confidence = _class_hang_confidence;
        }

        private string _id;
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _address;
        public string address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        private string _nationality;
        public string nationality
        {
            get
            {
                return _nationality;
            }
            set
            {
                _nationality = value;
            }
        }

        private string _due_date;
        public string due_date
        {
            get
            {
                return _due_date;
            }
            set
            {
                _due_date = value;
            }
        }

        private string _dob;
        public string dob
        {
            get
            {
                return _dob;
            }
            set
            {
                _dob = value;
            }
        }

        private string _issue_date;
        public string issue_date
        {
            get
            {
                return _issue_date;
            }
            set
            {
                _issue_date = value;
            }
        }

        private string _id_confidence;
        public string id_confidence
        {
            get
            {
                return _id_confidence;
            }
            set
            {
                _id_confidence = value;
            }
        }

        private string _name_confidence;
        public string name_confidence
        {
            get
            {
                return _name_confidence;
            }
            set
            {
                _name_confidence = value;
            }
        }

        private string _address_confidence;
        public string address_confidence
        {
            get
            {
                return _address_confidence;
            }
            set
            {
                _address_confidence = value;
            }
        }

        private string _nationality_confidence;
        public string nationality_confidence
        {
            get
            {
                return _nationality_confidence;
            }
            set
            {
                _nationality_confidence = value;
            }
        }

        private string _due_date_confidence;
        public string due_date_confidence
        {
            get
            {
                return _due_date_confidence;
            }
            set
            {
                _due_date_confidence = value;
            }
        }

        private string _dob_confidence;
        public string dob_confidence
        {
            get
            {
                return _dob_confidence;
            }
            set
            {
                _dob_confidence = value;
            }
        }

        private string _issue_date_confidence;
        public string issue_date_confidence
        {
            get
            {
                return _issue_date_confidence;
            }
            set
            {
                _issue_date_confidence = value;
            }
        }

        private string _class_hang;
        public string class_hang
        {
            get
            {
                return _class_hang;
            }
            set
            {
                _class_hang = value;
            }
        }

        private string _class_hang_confidence;
        public string class_hang_confidence
        {
            get
            {
                return _class_hang_confidence;
            }
            set
            {
                _class_hang_confidence = value;
            }
        }
    }


}
