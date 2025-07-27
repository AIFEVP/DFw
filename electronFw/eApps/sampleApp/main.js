console.info(`welcome to electron!`);

const { app, BrowserWindow } = require('electron');
const path = require('path');

function createWindow() {
    const win = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false,
        },
    });
    win.loadFile(path.join(__dirname, 'indexentry.html'));
    win.webContents.openDevTools();
}

console.info(`main process!`);

app.whenReady().then(createWindow);

