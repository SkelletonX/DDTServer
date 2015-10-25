

#include "stdafx.h"
#include <string>
#include <Windows.h>

using namespace System;
using namespace Lsj::Util;
using std::string;
using namespace Center::Server;
using namespace Game::Base;




void MarshalString(String ^ s, string& os);
void Start();
bool WINAPI ConsoleHandler(DWORD CEvent);
bool WINAPI StopServer();





int main(array<System::String^> ^args)
{
	if (args->Length>=1)
	{
		string key = "$^&^(*&)*(J1534765";
		string a = "";
		MarshalString(args[0], a);
		if (a == key)
		{
			Start();
		}
		else
		{
			WinForm::Notice("Error Key!");
		}
	}
	else
	{
		WinForm::Notice("Please run with the Launcher!");
	}
}

void Start()
{
	Console::Title = "DDTank Center Service";
	Console::WriteLine("Starting the Server ... Please Wait!");
	CenterServer::CreateInstance(gcnew CenterServerConfig());
	CenterServer::Instance->Start();
	SetConsoleCtrlHandler((PHANDLER_ROUTINE)ConsoleHandler, true);
	while (true)
	{
		System::Threading::Thread::Sleep(1);
	}
}

bool WINAPI ConsoleHandler(DWORD CEvent)
{
	switch (CEvent)
	{
	case CTRL_C_EVENT:
	case CTRL_BREAK_EVENT:
	case CTRL_CLOSE_EVENT:
	case CTRL_LOGOFF_EVENT:
	case CTRL_SHUTDOWN_EVENT:
		return StopServer();
	default:
		return FALSE;
	}
}
bool WINAPI StopServer()
{
	if (CenterServer::Instance)
	{
		CenterServer::Instance->Stop();
	}
	SetConsoleCtrlHandler((PHANDLER_ROUTINE)ConsoleHandler, false);
	return TRUE;
}

void MarshalString(String ^ s, string& os) {
	const char* chars =
		(const char*)(Runtime::InteropServices::Marshal::StringToHGlobalAnsi(s)).ToPointer();
	os = chars;
	Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr((void*)chars));
}
