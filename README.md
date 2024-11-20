# MySQL Backup Service

This Windows Service automatically backs up a MySQL database at regular intervals. It’s designed to be easy to install and configure for new users.

---

## **Features**
- Scheduled automatic MySQL database backups.
- Logs events for monitoring.
- Configurable backup intervals and target directories.

---

## **Requirements**
1. **Windows Operating System**
2. **MySQL Database Server**
   - Ensure the `mysqldump` tool is installed and added to your PATH environment variable.
3. **.NET Framework**
   - Version 4.0 or later (required for `InstallUtil.exe`).

---

## **Installation Instructions**

### **Step 1: Download the Files**
1. Clone the repository or download the latest release from the [Releases](https://github.com/darktrovx/Windows-MySQL-Backup-Service) page.
2. Ensure the following files are in the same directory:
   - `InstallService.ps1` (PowerShell installation script)
   - `Setup.exe` (the Windows Service executable)

---

### **Step 2: Configure the Service**
1. Open the service's configuration file (`app.config` or `.env`) if applicable.
2. Update the following fields:
   - **MySQL Connection Details**:
     ```plaintext
     DB_HOST=localhost
     DB_USER=root
     DB_PASSWORD=yourpassword
     DB_NAME=yourdatabase
     ```
   - **Backup Directory**:
     Specify where backups will be saved.
   - **Backup Interval** (if configurable):
     Adjust the interval for automatic backups.

---

### **Step 3: Install the Service**
1. **Run the PowerShell Installation Script**:
   Open PowerShell as **Administrator**, navigate to the folder containing the files, and run:
   ```powershell
    powershell -ExecutionPolicy Bypass -File InstallService.ps1
    ```

    This script will:
        Request admin privileges.
        Verify that the necessary tools (InstallUtil.exe) are available.
        Register the service with Windows.

    Verify Installation:
        Open the Services Manager (services.msc) and look for MySQL Backup Service.
        Ensure the service is set to Automatic start type.

Step 4: Start the Service

    Open Services Manager (services.msc).
    Find MySQL Backup Service in the list.
    Right-click the service and select Start.

## Updating the Service

### 1. To update the service executable:

Stop the service:
```powershell
    Stop-Service -Name "YourServiceName"
```

### 2. Replace the Setup.exe file with the updated version.

### 3. Restart the service:
```powershell
    Start-Service -Name "YourServiceName"
```

## Uninstalling the Service

### To uninstall the service:

### 1. Run the PowerShell script with the uninstall flag:
```powershell
    "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" /u Setup.exe
```

## Troubleshooting

###  1. Service Doesn't Appear in services.msc:

Ensure you ran the script with administrator privileges.

Check that InstallUtil.exe is available on your system.

###  2. Backup Fails:
Verify that the mysqldump command works manually.

Check the configuration file for correct MySQL credentials.

### 3. Logs
Check the service log files in the specified log directory for error details.

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvement, please open an issue or submit a pull request.