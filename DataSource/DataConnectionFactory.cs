using DataInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DataSource
{
    public static class DataConnectionFactory
    {
        public static IConnection CreateNewConnection(DataSourceType DT)
        {
            switch (DT)
            {
                case DataSourceType.LocalFile:
                    return new LocalFileConnection() { ConnectionName = string.Empty, ConnectionString = string.Empty, DSType = DT, SaveFormat =  SaveFormat.Json };               
                case DataSourceType.MSSQL:
                    SqlConnectionStringBuilder SB = new SqlConnectionStringBuilder();
                    SB.DataSource = "localhost";
                    SB.IntegratedSecurity = true;
                    SB.InitialCatalog = "EI_Database_2.01.11.7.500.74.7.500.111";
                    return new MSSQLConnection() { ConnectionName = "MSSQL DB", ConnectionString = SB.ToString(), DSType = DT };
                case DataSourceType.XMLDataset:
                    return new XMLDatasetConnection() { ConnectionName = "XML Dataset", ConnectionString = @"C:\Plexos stuff\Models\EI\EI_Database_2.01.11.xml", DSType = DT };
                case DataSourceType.RestService:
                    return new RESTConnection() { ConnectionName = "REST Dataset", ConnectionString = @"https://localhost:5001", DSType = DT };
            }
            return new LocalFileConnection() { };
        }
        public static IDataSource CreateNewDataSource(IConnection DC)
        {
            switch (DC.DSType)
            {
                case DataSourceType.LocalFile:
                    return new LocalFileDataSource(DC);               
                //case DataSourceType.MSSQL:
                //    SqlConnectionStringBuilder SB = new SqlConnectionStringBuilder();
                //    SB.DataSource = "localhost";
                //    SB.IntegratedSecurity = true;
                //    SB.InitialCatalog = "EI_Database_2.01.11.7.500.74.7.500.111";
                //    return new MSSQLDataSource(DC);
                //case DataSourceType.XMLDataset:
                //    return new XMLDatasetDataSource(DC);
                //case DataSourceType.RestService:
                //    return new RESTDataSource(DC);
            }
            return new LocalFileDataSource(DC);
        }

    }

}
