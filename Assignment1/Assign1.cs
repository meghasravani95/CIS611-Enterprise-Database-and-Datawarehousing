using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks; 
using System.Text.RegularExpressions;

namespace Queryprocessor
{   
    class Program 
    { 
        static void Main(string[] args) 
        { 
             /* Name: MEGHA SRAVANI LAVU */ 
            
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; 
            //Console.WriteLine(path); 
            string Emp_filepath = path + @"/Desktop/Assignment1/Employee.csv"; 
            DataTable EMPLOYEE = CSVtable(Emp_filepath); 
            string dep_filepath = path + @"/Desktop/Assignment1/department.csv"; 
            DataTable DEPARTMENT = CSVtable(dep_filepath); 
                    TableDisplay(EMPLOYEE);
                     //TableDisplay(DEPARTMENT);
            
            // variables used in program 
            string selection_tablename;
            string selection_column_name; 
            string selection_final_table; 
            DataTable step1table = new DataTable(); 
            string selection_number;
            string join_table2, join_col1, join_col2, join_final_table; 
            DataTable step2table = new DataTable(); 
            string[] projection = new string[10];
            int j=0;
            
            // Reading Input file from Input.txt 
            string line;
             string[] lines = new string[3];
            StreamReader sr = new StreamReader(path + @"/Desktop/Assignment1/Input.txt");
             while ((line = sr.ReadLine()) != null) 
             { 
                 string[] words = line.Split(' '); 
                 
                 for (int k = 0; k < words.Length; k++)
                  { 
                      if (words[0] == "Selection")
                       { 
                           k++; 
                           selection_tablename = words[1]; 
                           k++; 
                           selection_column_name = words[2]; 
                           k++;
                           k++; 
                           selection_number = words[4];
                            k++; 
                            selection_final_table = words[5];
                                //Console.WriteLine(selection_tablename+" "+selection_column_name+" "+selection_number+" "+selection_final_table); 
                            DataTable table = new DataTable(selection_final_table.ToString()); 
                            string emp = "EMPLOYEE";
                             string dep = "DEPARTMENT"; 
                             if (selection_tablename == emp) 
                                table = tableSelect(EMPLOYEE, selection_column_name, selection_number); 
                            else if (selection_tablename == dep) 
                                table = tableSelect(DEPARTMENT, selection_column_name, selection_number);
                                
                                //TableDisplay(table);
                                step1table = table.Copy();

                                TableDisplay(step1table);
                                  string step1_filepath = path + @"/Desktop/Assignment1/step1output.csv"; 
                                  ToCSVfile(step1table,step1_filepath);
                                    break;
                        } 

                        if (words[k] == "Join") 
                        { 
                            k++; 
                            k++; 
                            join_table2 = words[k]; 
                            k++; 
                            join_col1 = words[k]; 
                            k++; 
                            k++; 
                            join_col2 = words[k]; 
                            k++; 
                            join_final_table = words[k]; 
                            step2table = new DataTable(join_final_table.ToString());
                             string dep = "DEPARTMENT"; 
                             if (join_table2 == dep) 
                                step2table = tableJoin(step1table, DEPARTMENT, "DNO");
                                    
                                    TableDisplay(step2table);
                                    string step2_filepath = path + @"/Desktop/Assignment1/step2output.csv"; 
                                  ToCSVfile(step2table,step2_filepath);
                                     break;
                         } 

                         if (words[k] == "Projection")
                        {
                            k++; k++;
                            projection[j] = words[k];
                            j++; k++;
                            projection[j] = words[k];
                            j++; k++;
                            projection[j] = words[k];
                            j++; k++;
                            projection[j] = words[k];
                            j++; k++;
                            projection[j] = words[k];
                            j++; k++;
                            DataTable table = new DataTable(words[k]);
                            table = P_Result(step2table, projection);

                            TableDisplay(table); 
                            string step3_filepath = path + @"/Desktop/Assignment1/step3output.csv"; 
                                  ToCSVfile(table,step3_filepath);
                        }
                } 
             }
        }
        
        static DataTable CSVtable(string filepath)
        { 
            DataTable d= new DataTable();
             string[] lines=File.ReadAllLines(filepath);
            int i,j;
             string[] colnames= lines[0].Split(','); 
             
             foreach(string colname in colnames) 
             { 
                 d.Columns.Add(colname);
             }
                //Console.Write("linecount"+lines.length);
                
            for(i=0;i<lines.Length;i++)
             {
                  //Console.WriteLine(lines[i]); 
                  DataRow dr=d.NewRow(); 
                  string[] rowvalues = Regex.Split(lines[i],",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); 
                  for(j=0;j<colnames.Length;j++)
                  { 
                      dr[j]=rowvalues[j];
                  }
                   d.Rows.Add(dr);
             } 
             return d;
        }


        static void TableDisplay(DataTable table) 
        { 
            int i,j; 
 
            for (i=0;i< table.Rows.Count;i++) 
            { 
                for (j = 0; j <table.Columns.Count; j++) 
                { 
                    if(i==0 && j==0) 
                    { 
                        Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
                    } 
                    var cell=table.Rows[i][j]; 
                    Console.Write($"{cell,-15}");
                 } 
                Console.Write('\n');
            } 
        }

        static void ToCSVfile(DataTable table,string filepath)
        {
            //FileStream file=new FileStream(filepath,FileMode.OpenOrCreate,FileAccess.Write); 
            StringBuilder data=new StringBuilder();
            //StreamWriter sw=new StreamWriter(file); 
            int i,j;
                    for (i=0;i< table.Rows.Count;i++) 
                    { 
                        for (j = 0; j <table.Columns.Count; j++) 
                        { 
                            string cell=(table.Rows[i][j]).ToString(); 
                            //Console.Write(cell+",");
                           // sw.Write(cell+",");
                            if(j==table.Columns.Count-1)
                            {
                                data.Append(cell.Replace(",",","));
                            }
                            else
                            {
                                data.Append(cell.Replace(",",",")+',');
                            }
                           
                        }
                         //Console.WriteLine(); 
                         data.Append(Environment.NewLine);// for newline
                        // sw.Write("\n");
                     } 

                     using (StreamWriter sw = new StreamWriter(filepath))
                     {
                                 sw.WriteLine(data);
                    }

                   //  Console.Write("data"+data);

        } 
        static DataTable tableSelect(DataTable table,string selection_column_name,string selection_number) 
        { 
            DataTable d= new DataTable();
             int i,j; int colindex=-1,count=0; 
             // get colindex from the column names of the table 
             for(j=0;j<table.Columns.Count;j++) 
             { 
                 string colnames=(table.Rows[0][j]).ToString(); 
                 //Console.WriteLine(colnames); 
                 d.Columns.Add(colnames);
                  if(colnames==selection_column_name) 
                  { 
                      colindex=j;
                      
                      
                 }
             }
                // this is to select the rows which have selection number 
                for(i=0;i<table.Rows.Count;i++)
                 {

                      DataRow dr=d.NewRow(); 
                      for (j = 0; j <table.Columns.Count; j++) 
                      {
                           if((table.Rows[i][colindex]).ToString() == selection_number) 
                           { 
                               
                               dr[j]=table.Rows[i][j];
                               count=0;
                            }
                            else if(i==0)
                            {
                                dr[j]=table.Rows[i][j];
                                count=0;
                            }
                            else
                            {
                                count=1;
                            }

                        }
                       if(count==0)
                         d.Rows.Add(dr);
                          
                          
                        
                } 
            return d; 
         } 
         
         static DataTable tableJoin(DataTable ltable,DataTable rtable,string name) 
         { 
             

             DataTable d= new DataTable();
             int i,j;
             int dno=-1,rindex=-1;
             string rowvalue0=null,rowvalue2=null,rowvalue3=null;
             string colname1=null,colname2=null,colname3=null;

    
            
            for(i=0;i<ltable.Columns.Count;i++)
            {
                string lcolnames=(ltable.Rows[0][i]).ToString();
                //Console.WriteLine(lcolnames);
                d.Columns.Add(lcolnames);
                if(lcolnames==name)
                {
                    dno=Convert.ToInt32((ltable.Rows[1][i]));
                               
                }
            }


            
             for(i=0; i<rtable.Columns.Count; i++)
             {
                string rcolnames=(rtable.Rows[0][i]).ToString();
                 if(rcolnames != "Dnumber")
                 {
                    d.Columns.Add(rcolnames);
                   if(i==0)
                    colname1=rcolnames;
                    else if(i==2)
                     colname2=rcolnames;
                     else if(i==3)
                      colname3=rcolnames;
                         
                 }
                 else if(rcolnames == "Dnumber")
                 {
                     rindex=i;
                 }
                 
             }
                 

             for(i=1; i<rtable.Rows.Count; i++)
             {
                 // Console.WriteLine(rtable.Rows.Count);
                 if(dno==Convert.ToInt32((rtable.Rows[i][rindex])))
                 {
                      rowvalue0=(rtable.Rows[i][0]).ToString();
                      //Console.WriteLine(rowvalue0);
                      rowvalue2=(rtable.Rows[i][2]).ToString();
                      rowvalue3=(rtable.Rows[i][3]).ToString();
                     break;
                 }
                
             }
             


             for(i=0;i<ltable.Rows.Count;i++)
             {
                 DataRow dr=d.NewRow();
                 for(j=0;j<d.Columns.Count;j++)
                 {
                       if(j<ltable.Columns.Count)
                         dr[j]=ltable.Rows[i][j];
                      else if(j==ltable.Columns.Count && i!=0)
                         dr[j]=rowvalue0;
                      else if(j==ltable.Columns.Count+1 && i!=0)
                         dr[j]=rowvalue2;
                      else if(j==ltable.Columns.Count+2 && i!=0)
                         dr[j]=rowvalue3;
                         else if(i==0 && j==ltable.Columns.Count)
                          dr[j]=colname1;
                          else if(i==0 && j==ltable.Columns.Count+1)
                          dr[j]=colname2;
                          else if(i==0 && j==ltable.Columns.Count+2)
                          dr[j]=colname3;

                 }
                    
                d.Rows.Add(dr);
              
             }
             return d;

         } 

         static DataTable P_Result(DataTable table,String[] projection)
         {
             DataTable d=new DataTable();
            int[] index=new int[10];
            int i=0,c=0;

            //Console.WriteLine(projection.Length);
            for(int k=0;k<projection.Length;k++)
            {
                if(projection[k]!=null)
                {
                 c++;   
                }
            }
            //Console.WriteLine(c);

            for(int k=0;k<c;k++)
            {
                
                for(int j=0;j<table.Columns.Count;j++)
                {
                   
                    string row=(table.Rows[0][j]).ToString();
                    
                    if(row.ToUpper()==projection[k].ToUpper())
                    {
                        index[i]=j;
                        d.Columns.Add(row);
                        //Console.WriteLine(index[i]);
                        i++;
                    }
                }
            }

           for(int l=0;l<table.Rows.Count;l++)
           {
                DataRow dr=d.NewRow();
                 for(int m=0;m<d.Columns.Count;m++)
                 {
                     dr[m]=(table.Rows[l][index[m]]).ToString();
                 }
                 d.Rows.Add(dr);

           }
            return d;
         }
        
    }
 }