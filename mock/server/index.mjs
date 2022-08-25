import express from "express";
import multer from "multer";
import path from "path";
import { exec } from "child_process";
import { randomUUID } from "crypto";
import { createWriteStream } from "fs";

let app = express();

let fileStatus = {};
let queue = [];
let currentTask = null;

function predict(filename, model, callback) {
    let inFile = path.resolve(filename);
    let outFile = path.resolve(`output/${path.parse(filename).name}.png`);
    switch (model) {
        case "demoire":
            exec(`python app.py "${inFile}" "${outFile}"`, {
                cwd: "D:/source/DL/Screen_Image_Demoireing"
            }, callback); 
            break;
        default:
            callback("unknown model");
            break;
    }
}

function enqueue(filename, model) {
    function nextTask(error) {
        if (error) {
            fileStatus[currentTask[0]] = "failed";
            console.log(`${currentTask[0]} Failed with ${error}`);
        } else {
            fileStatus[currentTask[0]] = "done";
            console.log(`${currentTask[0]} Done`);

        }
        currentTask = null;
        if (queue.length != 0) {
            currentTask = queue.shift();
            predict(currentTask[0], currentTask[1], nextTask);
        }
    }
    if (currentTask == null) {
        currentTask = [filename, model];
        predict(currentTask[0], currentTask[1], nextTask);
    } else {
        queue.push([filename, model]);
    }
}

app.use(express.json());

app.post("/submit/demoire", function (req, res) {
    const token = `upload/${randomUUID()}.jpg`;
    console.log(`Upload file ${token}`);
    req.pipe(createWriteStream(token, { flags: 'w' }));
    req.on('end', () => {
        fileStatus[token] = "pending";
        enqueue(token, "demoire");
        res.json({
          code: 0,
          message: "success",
          data: {
            token
          },
        });
    })
});

app.get("/status", function (req, res) {
    const token = req.body.token;
    if (fileStatus[token] == 'pending') {
        res.json({
            code: 0,
            message: "success",
            data: {
                status: 0,
                squence: queue.length
            }
        });
    } else if (fileStatus[token] == 'done') {
        res.json({
            code: 0,
            message: "success",
            data: {
                status: 1,
                squence: 0
            }
        })
    } else {
        res.json({
            code: 1,
            message: "failed",
            data: null
        })
    }
});

app.get("/result", function(req, res) {
    const token = req.body.token;
    if (fileStatus[token] == "done") {
      res.sendFile(path.resolve(`output/${path.parse(token).name}.png`));
    } else {
      res.json({
        code: 1,
        message: "failed",
        data: null,
      });
    }
});

app.listen(8888, () => {
    console.log("listening on 8888");
});