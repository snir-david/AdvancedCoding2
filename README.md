


# Web Anomaly Server
### Preview from Website
![enter image description here](https://i.ibb.co/BPNBQMD/anoamlywebserver.png)
Our Website is made especially for pilots and flight researchers that want to explore flight data. 
Users can upload regular flight CSV (uses for learning the data and correlation) and another data flight CSV that we want to check for anomalies. 
The CSV files are upload to the server and return a table with anomalies that were detected. 
## Directory hierarchy

**Model**:
 - **Util.js**- useful function such as pearson, correlation and etc.
 - **hybrid and hybrid Detect.js** - using to learn data and detect anomalies using the hybrid algorithm.
 - **regression and regression  Detect.js** -  using to learn data and detect anomalies using regression algorithm.

 **Controller**:
- **expServer.js** - server itself. using express node.js module for the server.

**View**:
- **index.html** - this is the client-side view of the server. 
![enter image description here](https://siliconcanals.com/wp-content/uploads/2019/08/airplane-travel-tip.jpg)
## Installation and Running application
 - **For Developers** - install a proper IDE for Web Application (we used WebStorm but you can use any other software). You will need to install few programs and modules- 
 - [ ] **Node.js** - we worked with 14.17.0 version which you can download here - [Download | Node.js (nodejs.org)](https://nodejs.org/en/download/)
 - [ ] **npm** - install npm from here - [npm | get npm (npmjs.com)](https://www.npmjs.com/get-npm)
 - [ ] **express module** - in the command line enter - 
  `$ npm install express --save`
 for more information enter [here](https://expressjs.com/en/starter/installing.html).
 - [ ]  **express-fileupload module** - in the command line enter -
  `$ npm i express-fileupload`
for more information enter [here](https://www.npmjs.com/package/express-fileupload).
 - **For Users** -
 browse - 
 >  http://localhost:8080/
 
 upload files and wait until the result shows up.
## Documentation
Here you can find a Link to UML contains partial information of the central classes. UML represents the various connections between the classes and the most important information found in each class. UML can be found [here](https://lucid.app/lucidchart/7449e159-4b1b-4645-b7ae-9db34b7f0c98/view?page=0_0#). 
If you are a developer you can find full documentation of functions, variables, and more in the code.
## Video
Here you can find a link to our demo video - [link](https://youtu.be/6wdI9SiAkvA).
