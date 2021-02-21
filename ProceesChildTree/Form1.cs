using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Management;
using System.Collections.ObjectModel;

namespace ProceesChildTree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       

        /// <summary>
        /// Метод получения  родительского класса
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int GetParentProcessId(int id)
        {

            int parentId = 0;
            using (ManagementObject obj = new ManagementObject($"win32_process.handle={id}"))
            {
                try
                {
                    obj.Get();
                    parentId = Convert.ToInt32(obj["ParentProcessId"]);
                }
                catch (Exception)
                {

                    
                }
               
            }
            return parentId;
        }

        //эта перегрузка использовалась для дебага
        public TreeNode serchNode(string name)
        {
            var itemNode = new TreeNode();
            var findTreeNodes2 = treeView1.Nodes.Find(itemNode.Name, true);
            TreeNode node = null;
          
            foreach (var item in findTreeNodes2)
            {
                if (item.Text == name)
                {
                    node = item;
                   
                }
                
            }
            return node;
        }

        //удачно сломать мозг
        public void serchNode(Object tag,Process child)//tag это родитель
        {
            var itemNode = new TreeNode();
            var findTreeNodes2 = treeView1.Nodes.Find(itemNode.Name, true);//грузим всю индексацию дерева
       
            Process process = tag as Process;//этого можно было не делать но так проще отлаживать.Так что пометим это как МАГИЯ НЕ ТРОГАТЬ
            //если дерево пусто и родительский процесс не закрыт
            if (findTreeNodes2.Length==0 && process!=null)
            {//создаем родительский узел
                TreeNode newNode = new TreeNode();
                newNode.Tag = process;
                newNode.Text = process.ProcessName;
                //создаем его дочерний узел
                TreeNode newNode1 = new TreeNode();
                newNode1.Tag = child;
                newNode1.Text = child.ProcessName;
                //добавляем в родителя дочерний
                newNode.Nodes.Add(newNode1); 
                //добавляем весь узел 
                treeView1.Nodes.Add(newNode);
            }
            //если в дереве уже что то есть 
            else if(process!=null && findTreeNodes2.Length>0)
            {
                foreach (var item in findTreeNodes2)
                {
                    var n = item.Tag as Process;//так как у нас в узлах обьекты приводим тип к процессу
                    //ищем совпадение по родителььскому id 
                    if (process.Id == n.Id)//если текущий процесс дочерний то добавляем его 
                    {
                       
                        TreeNode newNode = new TreeNode();
                        newNode.Tag = child;
                        newNode.Text = child.ProcessName;
                        item.Nodes.Add(newNode);
                        break;
                    }
                    else//если родитель не найден то это говорит что процесс главный делаем его родителем а его процесс дочерним
                    {
                        TreeNode newNode = new TreeNode();
                        newNode.Tag = process;
                        newNode.Text = process.ProcessName;

                        TreeNode newNode1 = new TreeNode();
                        newNode1.Tag = child;
                        newNode1.Text = child.ProcessName;
                        newNode.Nodes.Add(newNode1);
                        treeView1.Nodes.Add(newNode);

                    }

                }
            }
           
          
        }
        /// <summary>
        /// метод определяет кто родитель процесса
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public Process GiveParent(Process process)
        {
            int parentId = 0;
            Process parent = null;
            using (ManagementObject obj = new ManagementObject($"win32_process.handle={process.Id}"))
            {
                try
                {
                    obj.Get();
                    parentId = Convert.ToInt32(obj["ParentProcessId"]);
                    parent = Process.GetProcessById(parentId);

                }
                catch 
                {


                }

            }
            return parent;

        }


        private void Form1_Load(object sender, EventArgs e)
        {

            Process[] process = Process.GetProcesses();

            //берем всего 20 процессов так как процессов очень много, а кофе заваривать я уже устал
            for (int i = 0; i < 20; i++)
            {
                Process parentFind = GiveParent(process[i]);
                serchNode(parentFind, process[i]);
            }


            //ВСЕ ЧТО НИЖЕ ПОПЫТКИ РЕШИТЬ ЗАДАЧУ 
            //ЧТО БЫ ИСКЛЮЧИТЬ МНЕНИЕ ЧТО СВИСНУЛ С ИНТЕРНЕТА
            //Напоминание как работать с деревом
            //TreeNode tovarNode = new TreeNode("Товары");
            //TreeNode tovarNode2 = new TreeNode("MyNode");
            //tovarNode.Nodes.Add("Какой то товар");
            //treeView1.Nodes.Add(tovarNode);
            //treeView1.Nodes.Add(tovarNode2);
            //treeView1.Nodes[0].Nodes[0].Nodes.Add("fdfdfd");

            //TreeNode[] n = treeView1.Nodes
            //                        .Cast<TreeNode>()
            //                        .Where(r => r.Text == "MyNode")
            //                        .ToArray();
            //var itemNode = new TreeNode();
            //var nodes = treeView1.Nodes;
            ////получаем колекцию всего дерева
            //var findTreeNodes = treeView1.Nodes.Find(itemNode.Name, true);

            //foreach (var item in findTreeNodes)
            //{
            //    if (item.Text=="MyNode")
            //    {
            //        item.Nodes.Add("jk");
            //    }
            //}

            //var findTreeNodes2 = treeView1.Nodes.Find(itemNode.Name, true);
            //foreach (var item in findTreeNodes2)
            //{
            //    if (item.Text == "jk")
            //    {
            //        item.Nodes.Add("ok");
            //    }
            //}

            //TreeNode node = serchNode("Товары");

            //node.Nodes.Add("1111");

            //вывожу список всех запущенных процессов в лист бокс для дебага
            // Process[] processes = Process.GetProcesses();//получаем список процессов 


            // получаем все диски на компьютере





            //foreach (var item in process)
            //{
            //    Process parentFind = GiveParent(item);
            //    //теперь ищем родителя в treeview
            //    serchNode(parentFind);
            //}



            //listBox1.Items.Add(Process.GetProcessById(8).ProcessName);

            //TreeNode node = null;
            //foreach (var item in process)
            //{


            //    node = new TreeNode();
            //    node.Tag = item;
            //    TreeNode serch = serchNode(node.Tag);
            //    node.Text = item.ProcessName;
            //    treeView1.Nodes.Add(node);
            //}




            //foreach (Process proc in process)
            //{

            //    int parentId = 0;
            //    using (ManagementObject obj = new ManagementObject("win32_process.handle=" + proc.Id.ToString()))
            //    {

            //        try
            //        {

            //            obj.Get();
            //            parentId = Convert.ToInt32(obj["ParentProcessId"]);


            //            listBox1.Items.Add($"{ parentId} {proc.ProcessName} {proc.Id} ");
            //        }
            //        catch
            //        {

            //        }



            //    }

            //}



        }
    }
}
