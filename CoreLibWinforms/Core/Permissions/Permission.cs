using CoreLibWinforms.Core.Permissions;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Permissions
{
    public class Permission
    {
        // 権限のID（これはBitArrayの位置としても使う）
        public int Id { get; set; }
        // 権限の名前
        public string Name { get; set; }

        public Permission(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
