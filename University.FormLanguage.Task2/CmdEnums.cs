using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.FormLanguage.Task2
{
	public enum EntryType 
	{ 
		Cmd,
		Var, 
		Const, 
		CmdPtr 
	}

	public enum Cmd 
	{ 
		JMP, 
		JZ, 
		SET,
		ADD, 
		SUB, 
		MUL, 
		DIV, 
		AND, 
		OR,
		CMPE, 
		CMPNE, 
		CMPL,  
		CMPLE,
		CMPG,
		CMPGE,
		PRINT 
	}

}
