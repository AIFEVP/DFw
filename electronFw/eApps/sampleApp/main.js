console.info(`welcome to electron!`);

const { app, BrowserWindow, ipcMain, webContents } = require('electron');
const path = require('path');
const windowStateKeeper = require('electron-window-state');

let win;
function createWindow() {
    const state = windowStateKeeper({
        height: 900,
        width: 600
    })
    win = new BrowserWindow({
        width: state.width,
        height: state.height,
        frame: false,
        backgroundColor: '#6f1818',
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false,
        },
    });


    // const nested = new BrowserWindow({
    //     width: 800,
    //     height: 900,
    //     // frame: false,
    //     backgroundColor: '#31778fff',
    //     webPreferences: {
    //         nodeIntegration: true,
    //         contextIsolation: false,
    //     },
    // });

    win.loadFile(path.join(__dirname, 'indexentry.html'));
    // nested.loadFile(path.join(__dirname, 'nestedyash.html'));
    win.webContents.openDevTools();
    console.info(`window created ${state.x} ${state.y}!`);

    state.manage(win);

    win.webContents.on('did-finish-load', () => {
        console.info('did-finish-load');
    });

    win.webContents.on('dom-ready', () => {
        console.info('dom-ready');
    });
}

console.info(`main process!`);

ipcMain.on('message', (event, message) => {
    console.info(message);
    event.reply('message', {msg: "hello from main, thank you!",
        time: new Date().toString(),
        pid: process.pid
    });
});


app.on('before-quit', () => {
    console.info('before-quit');
});

app.on('before-window-close', () => {
    console.info('before-window-close');
});

app.whenReady().then(createWindow);

