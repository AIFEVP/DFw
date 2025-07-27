console.info(`welcome to electron!`);

const { app, BrowserWindow } = require('electron');
const path = require('path');

function createWindow() {
    const win = new BrowserWindow({
        width: 800,
        height: 600,
        frame: false,
        backgroundColor: '#6f1818',
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false,
        },
    });


    const nested = new BrowserWindow({
        width: 800,
        height: 900,
        // frame: false,
        backgroundColor: '#31778fff',
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false,
        },
    });

    win.loadFile(path.join(__dirname, 'indexentry.html'));
    nested.loadFile(path.join(__dirname, 'nestedyash.html'));
    win.webContents.openDevTools();
}

console.info(`main process!`);

app.whenReady().then(createWindow);

